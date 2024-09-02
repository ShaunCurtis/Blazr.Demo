/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Infrastructure;

/// <summary>
/// This class provides list object mapping on top of the standard list handler
/// The out object is domain object
/// The in object is the Dbo object defining the record to retrieve from the data store
/// and should be defined in the DbContext
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <typeparam name="TDomainRecord">Domain Record to Map the incoming infrastructure record to</typeparam>
/// <typeparam name="TDatabaseRecord">This is the Dbo object the Handler will retrieve from the database</typeparam>
public class MappedListRequestServerHandler<TDbContext, TDomainRecord, TDatabaseRecord>
    : IListRequestHandler<TDomainRecord>
    where TDbContext : DbContext
    where TDatabaseRecord : class
    where TDomainRecord : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public MappedListRequestServerHandler(IDbContextFactory<TDbContext> factory, IServiceProvider serviceProvider)
    {
        _factory = factory;
        _serviceProvider = serviceProvider;
    }

    public virtual async ValueTask<ListQueryResult<TDomainRecord>> ExecuteAsync(ListQueryRequest request)
    {
        return await this.GetQueryAsync(request);
    }

    protected async ValueTask<ListQueryResult<TDomainRecord>> GetQueryAsync(ListQueryRequest request)
    {
        int totalRecordCount = 0;

        IRecordSortHandler<TDatabaseRecord>? sorterHandler = null;
        IRecordFilterHandler<TDatabaseRecord>? filterHandler = null;

        // Get and check we havw a mapper for the Dbo object to Dco Domain Model
        IDboEntityMap<TDatabaseRecord, TDomainRecord>? mapper = null;
        mapper = _serviceProvider.GetService<IDboEntityMap<TDatabaseRecord, TDomainRecord>>();

        // Throw an exception if we have no mapper defined 
        if (mapper is null)
            throw new DataPipelineException($"No mapper is defined for {this.GetType().FullName} for {(typeof(TDatabaseRecord).FullName)}");

        // Get a Unit of Work DbContext for the scope of the method
        using var dbContext = _factory.CreateDbContext();
        // Turn off tracking.  We're only querying, no changes
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Get the IQueryable DbSet for TRecord
        IQueryable<TDatabaseRecord> inQuery = dbContext.Set<TDatabaseRecord>();

        // If we have filters defined we need to get the Filter Handler for TRecord
        // and apply the predicate delegates to the IQueryable instance
        if (request.Filters.Count() > 0)
        {
            // Get the Record Filter
            filterHandler = _serviceProvider.GetService<IRecordFilterHandler<TDatabaseRecord>>();

            // Throw an exception as we have filters defined, but no handler 
            if (filterHandler is null)
                throw new DataPipelineException($"Filters are defined in {this.GetType().FullName} for {(typeof(TDatabaseRecord).FullName)} but no FilterProvider service is registered");

            // Apply the filters
            inQuery = filterHandler.AddFiltersToQuery(request.Filters, inQuery);
        }

        // Get the total record count after applying the filters
        totalRecordCount = inQuery is IAsyncEnumerable<TDatabaseRecord>
            ? await inQuery.CountAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None)
            : inQuery.Count();

        // If we have sorters we need to gets the Sort Handler for TRecord
        // and apply the sorters to thw IQueryable instance
        if (request.Sorters.Count() > 0)
        {
            sorterHandler = _serviceProvider.GetService<IRecordSortHandler<TDatabaseRecord>>();

            if (sorterHandler is null)
                throw new DataPipelineException($"Sorters are defined in {this.GetType().FullName} for {(typeof(TDatabaseRecord).FullName)} but no SorterProvider service is registered");

            inQuery = sorterHandler.AddSortsToQuery(inQuery, request.Sorters);
        }

        // Apply paging to the filtered and sorted IQueryable
        if (request.PageSize > 0)
            inQuery = inQuery
                .Skip(request.StartIndex)
                .Take(request.PageSize);

        // Apply the mapping to the query
        var outQuery = inQuery.Select(item => mapper.MapTo(item));

        // Materialize the out list from the data source
        var list = outQuery is IAsyncEnumerable<TDomainRecord>
            ? await outQuery.ToListAsync().ConfigureAwait(ConfigureAwaitOptions.None)
            : outQuery.ToList();

        return ListQueryResult<TDomainRecord>.Success(list, totalRecordCount);
    }
}