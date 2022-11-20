/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherForecastEditContext
    : RecordEditContextBase<DboWeatherForecast>, IEditRecord<DboWeatherForecast>, IMessageStoreValidation, IAuthRecord
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

    private DateTimeOffset _date;
    public DateTimeOffset Date
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

    public override ValidationResult Validate(string? fieldname = null)
    {
        var result = WeatherForecastValidator.Validate(this.Record, ValidationMessages, fieldname);
        this.NotifyValidationStateUpdated(result.IsValid, fieldname);
        return result;
    }


    public bool Validate(ValidationMessageStore? validationMessageStore, string? fieldname, object? model = null)
    {
        model ??= this;
        ValidationState validationState = new ValidationState();
        var field = "Date";

        field = "Date";
        if (field.Equals(fieldname) | fieldname is null)
            this.Date.Validation(field, model, validationMessageStore, validationState)
                .GreaterThan(DateTime.Now, true, "The weather forecast must be for a future date")
                .Validate(fieldname);

        field = "SummaryId";
        if (field.Equals(fieldname) | fieldname is null)
            this.SummaryId.Validation(field, model, validationMessageStore, validationState)
            .NotEmpty("You must select a weather summary")
            .Validate(fieldname);

        field = "LocationId";
        if (field.Equals(fieldname) | fieldname is null)
            this.LocationId.Validation(field, model, validationMessageStore, validationState)
            .NotEmpty("You must select a location")
            .Validate(fieldname);

        field = "TemperatureC";
        if (field.Equals(fieldname) | fieldname is null)
            this.TemperatureC.Validation(field, model, validationMessageStore, validationState)
            .GreaterThan(-61, "The minimum Temperatore is -60C")
            .LessThan(61, "The maximum temperature is 60C")
            .Validate(fieldname);

        return validationState.IsValid;
    }
}
