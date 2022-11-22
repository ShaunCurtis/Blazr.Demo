/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherForecastEditContext
    : RecordEditContextBase<DboWeatherForecast>, IMessageCollectionValidation, IAuthRecord
{
    private Guid _newId = Guid.NewGuid();

    private Guid _uid = Guid.Empty;
    public override Guid Uid
    {
        get => _uid;
        set => UpdateifChangedAndNotify(ref _uid, value, this.BaseRecord.Uid, WeatherForecastConstants.Uid);
    }

    private Guid _summaryId = Guid.Empty;
    public Guid SummaryId
    {
        get => _summaryId;
        set => UpdateifChangedAndNotify(ref _summaryId, value, this.BaseRecord.WeatherSummaryId, WeatherForecastConstants.SummaryId);
    }

    private Guid _locationId = Guid.Empty;
    public Guid LocationId
    {
        get => _locationId;
        set => UpdateifChangedAndNotify(ref _locationId, value, this.BaseRecord.WeatherLocationId, WeatherForecastConstants.LocationId);
    }

    private DateOnly _date;
    public DateOnly Date
    {
        get => _date;
        set => UpdateifChangedAndNotify(ref _date, value, this.BaseRecord.Date, WeatherForecastConstants.Date);
    }

    private Guid _ownerId = Guid.Empty;
    public Guid OwnerId
    {
        get => _ownerId;
        set => UpdateifChangedAndNotify(ref _ownerId, value, this.BaseRecord.OwnerId, WeatherForecastConstants.OwnerId);
    }

    private int _temperatureC;
    public int TemperatureC
    {
        get => _temperatureC;
        set => UpdateifChangedAndNotify(ref _temperatureC, value, this.BaseRecord.TemperatureC, WeatherForecastConstants.TemperatureC);
    }

    public WeatherForecastEditContext()
    {
        var rec = new DboWeatherForecast();
        this.Load(rec);
    }

    public WeatherForecastEditContext(DboWeatherForecast item)
        => this.Load(item);

    public override void Load(DboWeatherForecast record, bool notify = true)
    {
        this.BaseRecord = record with { };

        this.Uid = record.Uid;
        this.SummaryId = record.WeatherSummaryId;
        this.LocationId = record.WeatherLocationId;
        this.OwnerId = record.OwnerId;
        this.Date = record.Date;
        this.TemperatureC = record.TemperatureC;

        if (notify)
            this.NotifyFieldChanged(null);
    }

    public override DboWeatherForecast Record =>
        new DboWeatherForecast()
        {
            Uid = this.Uid,
            WeatherSummaryId = this.SummaryId,
            WeatherLocationId = this.LocationId,
            OwnerId = this.OwnerId,
            Date = this.Date,
            TemperatureC = this.TemperatureC
        };

    public override DboWeatherForecast AsNewRecord()
        => new DboWeatherForecast()
        {
            Uid = _newId,
            WeatherSummaryId = this.SummaryId,
            WeatherLocationId = this.LocationId,
            OwnerId = this.OwnerId,
            Date = this.Date,
            TemperatureC = this.TemperatureC
        };

    public override ValidationResult Validate(FieldReference? field = null)
    {
        var result = WeatherForecastValidator.Validate(this.Record, ValidationMessages, field);
        this.NotifyValidationStateUpdated(result.IsValid, field);
        return result;
    }
}
