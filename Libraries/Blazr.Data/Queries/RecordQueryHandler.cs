/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Data;

public class RecordQueryHandler<TRecord, TDbContext>
    : ICQSHandler<RecordQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext
{
    private readonly RecordQuery<TRecord> _query;
    private IDbContextFactory<TDbContext> _factory;
    private bool _success = true;
    private string _message = string.Empty;

    public RecordQueryHandler(IDbContextFactory<TDbContext> factory, RecordQuery<TRecord> query)
    {
        _factory = factory;
        _query = query;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync()
    {
        var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        TRecord? record = null;

        // first check if the record implements IRecord.  If so we can do a cast and then do the query via the Uid property directly 
        if ((new TRecord()) is IRecord)
            record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Uid == _query.GuidId);

        // Try and use the EF FindAsync implementation
        if (record == null)
        {
            if (_query.GuidId != Guid.Empty)
                record = await dbContext.FindAsync<TRecord>(_query.GuidId);

            if (_query.LongId > 0)
                record = await dbContext.FindAsync<TRecord>(_query.LongId);

            if (_query.IntId > 0)
                record = await dbContext.FindAsync<TRecord>(_query.IntId);
        }

        if (record is null)
        {
            _message = "No record retrieved";
            _success = false;
        }

        return new RecordProviderResult<TRecord>(record, _success, _message);
    }
}
