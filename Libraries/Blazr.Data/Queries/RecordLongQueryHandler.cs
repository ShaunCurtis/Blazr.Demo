/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Data;

public class RecordLongQueryHandler<TRecord, TDbContext>
    : ICQSHandler<RecordGuidKeyQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext

{
    private readonly RecordLongKeyQuery<TRecord> _query;
    private IDbContextFactory<TDbContext> _factory;
    private bool _success = true;
    private string _message = string.Empty;

    public RecordLongQueryHandler(IDbContextFactory<TDbContext> factory, RecordLongKeyQuery<TRecord> query)
    {
        _factory = factory;
        _query = query;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync()
    {
        var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        TRecord? record = null;

        // first check if the record implements IRecord.  If so we can do a cast and then do the quesry via the Id property directly 
        //if ((new TRecord()) is IRecord)
        //    record = await _dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Id == _query.RecordId);

        // Try and use the EF FindAsync implementation
        if (record == null)
            record = await dbContext.FindAsync<TRecord>(_query.RecordId);

        if (record is null)
        {
            _message = "No record retrieved";
            _success = false;
        }

        return new RecordProviderResult<TRecord>(record, _success, _message);
    }
}
