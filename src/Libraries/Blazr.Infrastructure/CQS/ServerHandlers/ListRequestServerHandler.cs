/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Infrastructure;

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
        where TRecord : class, new()
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
    where TRecord : class, new()
    {
        int count = 0;
        if (request == null)
            throw new DataPipelineException($"No ListQueryRequest defined in {this.GetType().FullName}");

        var sorterProvider = _serviceProvider.GetService<IRecordSorter<TRecord>>();
        var filterProvider = _serviceProvider.GetService<IRecordFilter<TRecord>>();

        if(request.Filters.Count() > 0 && filterProvider is null )
            throw new DataPipelineException($"Filters are defined in {this.GetType().FullName} for {(new TRecord().GetType().FullName)} but no FilterProvider service is registered");

        if (request.Sorters.Count() > 0 && sorterProvider is null)
            throw new DataPipelineException($"Sorters are defined in {this.GetType().FullName} for {(new TRecord().GetType().FullName)} but no SorterProvider service is registered");

        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();
        if (filterProvider is not null)
            query = filterProvider.AddFilterToQuery(request.Filters, query);

        count = query is IAsyncEnumerable<TRecord>
            ? await query.CountAsync(request.Cancellation)
            : query.Count();

        if (sorterProvider is not null)
            query = sorterProvider.AddSortToQuery(query, request.Sorters);

        if (request.PageSize > 0)
            query = query
                .Skip(request.StartIndex)
                .Take(request.PageSize);

        var list = query is IAsyncEnumerable<TRecord>
            ? await query.ToListAsync()
            : query.ToList();

        return ListQueryResult<TRecord>.Success(list, count);
    }
}