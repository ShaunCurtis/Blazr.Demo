/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Data;

public class RecordQueryHandler<TRecord, TDbContext>
    : IHandler<RecordQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext
{
    private IDbContextFactory<TDbContext> _factory;
    private bool _success = true;
    private string _message = string.Empty;

    public RecordQueryHandler(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync(RecordQuery<TRecord> query)
    {
        var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        TRecord? record = null;

        // first check if the record implements IRecord.  If so we can do a cast and then do the query via the Uid property directly 
        if ((new TRecord()) is IRecord)
            record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Uid == query.GuidId, query.CancellationToken);

        // Try and use the EF FindAsync implementation
        if (record == null)
        {
            if (query.GuidId != Guid.Empty)
                record = await dbContext.FindAsync<TRecord>(query.GuidId, query.CancellationToken);

            if (query.LongId > 0)
                record = await dbContext.FindAsync<TRecord>(query.LongId, query.CancellationToken);

            if (query.IntId > 0)
                record = await dbContext.FindAsync<TRecord>(query.IntId, query.CancellationToken);
        }

        if (record is null)
        {
            _message = "No record retrieved";
            _success = false;
        }

        return new RecordProviderResult<TRecord>(record, _success, _message);
    }
}
