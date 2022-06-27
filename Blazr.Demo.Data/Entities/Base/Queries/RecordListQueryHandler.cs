/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public class RecordListQueryHandler<TRecord, TDbContext>
    : ICQSHandler<RecordListQuery<TRecord>, ValueTask<ListProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext, IWeatherDbContext

{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;

    protected IDbContextFactory<TDbContext> factory;
    protected readonly RecordListQuery<TRecord> listQuery;

    public RecordListQueryHandler(IDbContextFactory<TDbContext> factory, RecordListQuery<TRecord> query)
    {
        this.factory = factory;
        this.listQuery = query;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<ListProviderResult<TRecord>> _executeAsync()
    {
        if (this.listQuery is null)
            return new ListProviderResult<TRecord>(new List<TRecord>(), 0);
        if (await this.GetItemsAsync())
            await this.GetCountAsync();
        return new ListProviderResult<TRecord>(this.items, this.count);
    }

    protected virtual async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        if (listQuery is null)
            return false;

        IQueryable<TRecord> dbSet = dbContext.Set<TRecord>();

        if (listQuery.Request.PageSize > 0)
            dbSet = dbSet
                .Skip(listQuery.Request.StartIndex)
                .Take(listQuery.Request.PageSize);

        this.items = await dbSet.ToListAsync();

        return true;
    }

    protected virtual async ValueTask<bool> GetCountAsync()
    {
        var dbContext = this.factory.CreateDbContext();

        if (listQuery is null)
            return false;

        IQueryable<DvoWeatherForecast> dbSet = dbContext.Set<DvoWeatherForecast>();

        count = await dbSet.CountAsync();
        return true;
    }

}
