/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class DeoWeatherForecast
    : IEditRecord<DboWeatherForecast>, IValidation, IAuthRecord
{
    private DboWeatherForecast _baseRecord = new DboWeatherForecast();
    private Guid _newId = Guid.NewGuid();

    public Guid Uid { get; set; } = GuidExtensions.Null;

    public Guid SummaryId { get; set; }

    public Guid LocationId { get; set; }

    public DateTimeOffset Date { get; set; }

    public Guid OwnerId { get; set; } = Guid.Empty;

    public int TemperatureC { get; set; }

    public bool IsNull => Uid == GuidExtensions.Null;

    public bool IsNew => Uid == Guid.Empty;

    public bool IsDirty => _baseRecord != this.Record;

    public DboWeatherForecast CleanRecord => _baseRecord;

    public DeoWeatherForecast()
    {
        var rec = new DboWeatherForecast();
        this.Load(rec);
    }

    public DeoWeatherForecast(DboWeatherForecast item)
        => this.Load(item);

    public void Reset()
    => this.Load(_baseRecord with { });

    public void Load(DboWeatherForecast record)
    {
        _baseRecord = record with { };

        this.Uid = record.Uid;
        this.SummaryId = record.WeatherSummaryId;
        this.LocationId = record.WeatherLocationId;
        this.OwnerId = record.OwnerId;
        this.Date = record.Date;
        this.TemperatureC = record.TemperatureC;
    }

    public DboWeatherForecast Record =>
        new DboWeatherForecast()
        {
            Uid = this.Uid,
            WeatherSummaryId = this.SummaryId,
            WeatherLocationId = this.LocationId,
            OwnerId = this.OwnerId,
            Date = this.Date,
            TemperatureC = this.TemperatureC
        };

    public DboWeatherForecast AsNewRecord =>
        new DboWeatherForecast()
        {
            Uid = _newId,
            WeatherSummaryId = this.SummaryId,
            WeatherLocationId = this.LocationId,
            OwnerId = this.OwnerId,
            Date = this.Date,
            TemperatureC = this.TemperatureC
        };

    public bool Validate(ValidationMessageStore? validationMessageStore, string? fieldname, object? model = null)
    {
        model ??= this;
        ValidationState validationState = new ValidationState();

        this.Date.Validation("Date", model, validationMessageStore, validationState)
            .GreaterThan(DateTime.Now, true, "The weather forecast must be for a future date")
            .Validate(fieldname);

        this.SummaryId.Validation("SummaryId", model, validationMessageStore, validationState)
            .NotEmpty("You must select a weather summary")
            .Validate(fieldname);

        this.LocationId.Validation("LocationId", model, validationMessageStore, validationState)
            .NotEmpty("You must select a location")
            .Validate(fieldname);

        this.TemperatureC.Validation("TemperatureC", model, validationMessageStore, validationState)
            .GreaterThan(-61, "The minimum Temperatore is -60C")
            .LessThan(81, "The maximum temperature is 80C")
            .Validate(fieldname);

        return validationState.IsValid;
    }
}
