﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

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
        => await _executeAsync();

    private async ValueTask<RecordProviderResult<TRecord>> _executeAsync()
    {
        var _dbContext = _factory.CreateDbContext();
        TRecord? record = null;
        if (GetKeyProperty(out PropertyInfo? value) && value is not null)
        {
            record = await _dbContext.Set<TRecord>()
                .SingleOrDefaultAsync(item => GuidCompare(value.GetValue(item)));

            if (record is null)
            {
                _message = "No record retrieved";
                _success = false;
            }
        }
        return new RecordProviderResult<TRecord>(record, _success, _message);
    }

    private bool GuidCompare(object? value)
        => value is Guid && (Guid)value == _query.RecordId;

    private bool GetKeyProperty(out PropertyInfo? value)
    {
        var instance = new TRecord();
        value = instance.GetType()
            .GetProperties()
            .FirstOrDefault(prop => prop.GetCustomAttributes(false)
                .OfType<KeyAttribute>()
                .Any());
        if (value is null)
        {
            _message = "No Key attribute defined for the data set";
            _success = false;
        }

        return value is not null;
    }
}
