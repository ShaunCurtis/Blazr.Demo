/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherForecastEditContext : RecordEditContextBase<DboWeatherForecast>
{
    private Guid _newId = Guid.NewGuid();

    private Guid _uid = Guid.Empty;
    public Guid Uid
    {
        get => _uid;
        set => SetIfChanged(ref _uid, value, WeatherForecastConstants.Uid);
    }

    private Guid _ownerId = Guid.Empty;
    public Guid OwnerId
    {
        get => _ownerId;
        set => SetIfChanged(ref _uid, value, WeatherForecastConstants.OwnerId);
    }

    private Guid _summaryId = Guid.Empty;
    public Guid SummaryId
    {
        get => _summaryId;
        set => SetIfChanged(ref _uid, value, WeatherForecastConstants.SummaryId);
    }

    private Guid _locationId = Guid.Empty;
    public Guid LocationId
    {
        get => _locationId;
        set => SetIfChanged(ref _uid, value, WeatherForecastConstants.LocationId);
    }

    private DateTimeOffset _date;
    public DateTimeOffset Date
    {
        get => _date;
        set => SetIfChanged(ref _date, value, WeatherForecastConstants.Date);
    }

    private int _temperatureC;

    public int TemperatureC
    {
        get => _temperatureC;
        set => SetIfChanged(ref _temperatureC, value, WeatherForecastConstants.Date);
    }

    public WeatherForecastEditContext(DboWeatherForecast record) : base(record) { }

    public override void Load(DboWeatherForecast record)
    {
        this.BaseRecord = record with { };
        _uid = record.Uid;
        _ownerId = record.OwnerId;
        _summaryId = record.WeatherSummaryId;
        _locationId = record.WeatherLocationId;
        _date = record.Date;
        _temperatureC = record.TemperatureC;
    }
    public override void Reset()
        => this.Load(this.BaseRecord with { });

    public DboWeatherForecast AsNewRecord
        => CurrentRecord with { Uid = _newId };

    public override DboWeatherForecast CurrentRecord
        => new DboWeatherForecast
        {
            Uid = _uid,
            OwnerId = _ownerId,
            WeatherSummaryId = _summaryId,
            WeatherLocationId = _locationId,
            Date = _date,
            TemperatureC = _temperatureC
        };

    public override ValidationResult Validate(string? fieldname = null)
    {
        var result = WeatherForecastValidator.Validate(this.CurrentRecord, ValidationMessages, fieldname);
        this.NotifyValidationStateUpdated(result.IsValid, fieldname);
        return result;
    }
}
