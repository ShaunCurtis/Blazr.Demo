/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public sealed class ListRequestServerHandler<TDbContext>
    : IListRequestHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public ListRequestServerHandler(IDbContextFactory<TDbContext> factory, IServiceProvider serviceProvider)
    {
        _factory = factory;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class
    {
        IListRequestHandler<TRecord>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<IListRequestHandler<TRecord>>();

        // If we get one then one is registered in DI and we execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If there's no custom handler registered we run the base handler
        return await this.GetItemsAsync<TRecord>(request);
    }

    private async ValueTask<ListQueryResult<TRecord>> GetItemsAsync<TRecord>(ListQueryRequest request)
    where TRecord : class
    {
        int totalRecordCount = 0;

        IRecordSortHandler<TRecord>? sorterHandler = null;
        IRecordFilterHandler<TRecord>? filterHandler = null;

        // Get a Unit of Work DbContext for the scope of the method
        using var dbContext = _factory.CreateDbContext();
        // Turn off tracking.  We're only querying, no changes
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Get the IQueryable DbSet for TRecord
        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        // If we have filters defined we need to get the Filter Handler for TRecord
        // and apply the predicate delegates to the IQueryable instance
        if (request.Filters.Count() > 0)
        {
            // Get the Record Filter
            filterHandler = _serviceProvider.GetService<IRecordFilterHandler<TRecord>>();

            // Throw an exception as we have filters defined, but no handler 
            if (filterHandler is null)
                throw new DataPipelineException($"Filters are defined in {this.GetType().FullName} for {(typeof(TRecord).FullName)} but no FilterProvider service is registered");

            // Apply the filters
            query = filterHandler.AddFiltersToQuery(request.Filters, query);
        }

        // Get the total record count after applying the filters
        totalRecordCount = query is IAsyncEnumerable<TRecord>
            ? await query.CountAsync(request.Cancellation)
            : query.Count();

        // If we have sorters we need to gets the Sort Handler for TRecord
        // and apply the sorters to thw IQueryable instance
        if (request.Sorters.Count() > 0)
        {
            sorterHandler = _serviceProvider.GetService<IRecordSortHandler<TRecord>>();

            if (sorterHandler is null)
                throw new DataPipelineException($"Sorters are defined in {this.GetType().FullName} for {(typeof(TRecord).FullName)} but no SorterProvider service is registered");

            query = sorterHandler.AddSortsToQuery(query, request.Sorters);
        }

        // Apply paging to the filtered and sorted IQueryable
        if (request.PageSize > 0)
            query = query
                .Skip(request.StartIndex)
                .Take(request.PageSize);

        // Finally materialize the list from the data source
        var list = query is IAsyncEnumerable<TRecord>
            ? await query.ToListAsync()
            : query.ToList();

        return ListQueryResult<TRecord>.Success(list, totalRecordCount);
    }
}