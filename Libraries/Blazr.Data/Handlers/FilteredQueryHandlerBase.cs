/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Data;

public class FilteredListQueryHandlerBase<TRecord, TDbContext>
    : IFilteredListQueryHandler<TRecord>
        where TDbContext : DbContext
        where TRecord : class, new()

{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;

    protected IDbContextFactory<TDbContext> factory;
    protected IFilteredListQuery<TRecord> listQuery = default!;

    public FilteredListQueryHandlerBase(IDbContextFactory<TDbContext> factory)
    {
        this.factory = factory;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync(IFilteredListQuery<TRecord> query)
    {
        if (query is null)
            return new ListProviderResult<TRecord>(new List<TRecord>(), 0, false, "No Query Defined");

        listQuery = query;

        if (await this.GetItemsAsync())
            await this.GetCountAsync();

        return new ListProviderResult<TRecord>(this.items, this.count);
    }

    protected virtual async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();
        if (listQuery.Request.FilterExpression is not null)
            query = query
                .Where(listQuery.Request.FilterExpression)
                .AsQueryable();

        if (listQuery.Request.SortExpressionString is not null)
            query = query.OrderBy(listQuery.Request.SortExpressionString);

        if (listQuery.Request.PageSize > 0)
            query = query
                .Skip(listQuery.Request.StartIndex)
                .Take(listQuery.Request.PageSize);

        if (query is IAsyncEnumerable<TRecord>)
            this.items = await query.ToListAsync();
        else
            this.items = query.ToList();

        return true;
    }

    protected virtual async ValueTask<bool> GetCountAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();
        if (listQuery.Request.FilterExpression is not null)
            query = query.Where(listQuery.Request.FilterExpression).AsQueryable();

        if (query is IAsyncEnumerable<TRecord>)
            count = await query.CountAsync();
        else
            count = query.Count();

        return true;
    }
}
