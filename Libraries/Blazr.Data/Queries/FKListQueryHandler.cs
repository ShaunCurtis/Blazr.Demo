/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Data;

public sealed class FKListQueryHandler<TRecord, TDbContext>
    : IHandlerAsync<FKListQuery<TRecord>, ValueTask<FKListProviderResult<TRecord>>>
        where TDbContext : DbContext
        where TRecord : class, IFkListItem, new()
{
    private IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    private readonly IDbContextFactory<TDbContext> factory;

    public FKListQueryHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<FKListProviderResult<TRecord>> ExecuteAsync(FKListQuery<TRecord> listQuery)
    {
        using var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        if (listQuery is null)
            return FKListProviderResult<TRecord>.Failure("No Query defined");

        IEnumerable<TRecord> dbSet = await dbContext.Set<TRecord>().ToListAsync(listQuery.CancellationToken);

        return FKListProviderResult<TRecord>.Successful(dbSet);
    }
}
