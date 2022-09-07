﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Data;

public class FKListQueryHandler<TRecord, TDbContext>
    : IHandler<FKListQuery<TRecord>, ValueTask<FKListProviderResult>>
        where TDbContext : DbContext
        where TRecord : class, IFkListItem, new()
{
    protected IDbContextFactory<TDbContext> factory;

    public FKListQueryHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<FKListProviderResult> ExecuteAsync(FKListQuery<TRecord> query)
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IEnumerable<TRecord> dbSet = await dbContext.Set<TRecord>().ToListAsync(query.CancellationToken);

        return new FKListProviderResult(dbSet);
    }
}
