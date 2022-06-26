/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public abstract class RecordQueryHandlerBase<TRecord>
    : IRequestHandler<RecordQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
    where TRecord : class, new()
{
    private readonly IWeatherDbContext _dbContext;
    private readonly RecordQuery<TRecord> _query;

    public RecordQueryHandlerBase(IWeatherDbContext dbContext, RecordQuery<TRecord> query)
    {
        _dbContext = dbContext;
        _query = query;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<RecordProviderResult<TRecord>> _executeAsync()
    {
        TRecord? record = null;
        if (GetKeyProperty(out PropertyInfo? value) && value is not null)
        {
            record = await _dbContext.Set<TRecord>()
                .SingleOrDefaultAsync(item => GuidCompare(value.GetValue(item)));
        }
        return new RecordProviderResult<TRecord>(record);
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

        return value is not null;
    }
}
