/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Infrastructure;

public sealed class RecordQueryHandler<TRecord, TDbContext>
    : IHandlerAsync<RecordQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public RecordQueryHandler(IDbContextFactory<TDbContext> factory)
        =>  _factory = factory;

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync(RecordQuery<TRecord> query)
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        TRecord? record = null;

        // first check if the record implements IRecord.  If so we can do a cast and then do the query via the Uid property directly 
        if ((new TRecord()) is IRecord)
            record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Uid == query.Uid, query.CancellationToken);

        // Try and use the EF FindAsync implementation
        if (record is null)
                record = await dbContext.FindAsync<TRecord>(query.Uid);

        if (record is null)
            return RecordProviderResult<TRecord>.Failure("No record retrieved");

        return RecordProviderResult<TRecord>.Successful(record);
    }
}
