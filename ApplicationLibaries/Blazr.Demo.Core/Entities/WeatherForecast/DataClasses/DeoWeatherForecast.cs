/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Core;

public class DeoWeatherForecast
    : IEditRecord<DboWeatherForecast>, IValidation
{
    private DboWeatherForecast _baseRecord = new DboWeatherForecast();
    private Guid _newId = Guid.NewGuid();

    public Guid Id { get; set; } = GuidExtensions.Null;

    public Guid SummaryId { get; set; }

    public DateTimeOffset Date { get; set; }

    public int TemperatureC { get; set; }

    public bool IsNull => Id == GuidExtensions.Null;

    public bool IsNew => Id == Guid.Empty;

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

        this.Id = record.WeatherForecastId;
        this.SummaryId = record.WeatherSummaryId;
        this.Date = record.Date;
        this.TemperatureC = record.TemperatureC;
    }

    public DboWeatherForecast Record =>
        new DboWeatherForecast()
        {
            WeatherForecastId = this.Id,
            WeatherSummaryId = this.SummaryId,
            Date = this.Date,
            TemperatureC = this.TemperatureC
        };

    public DboWeatherForecast AsNewRecord =>
        new DboWeatherForecast()
        {
            WeatherForecastId = _newId,
            WeatherSummaryId = this.SummaryId,
            Date = this.Date,
            TemperatureC = this.TemperatureC
        };

    public bool Validate(ValidationMessageStore? validationMessageStore, string? fieldname, object? model = null)
    {
        model = model ?? this;
        bool trip = false;

        this.Date.Validation("Date", model, validationMessageStore)
            .LessThan(DateTime.Now.AddDays(10))
            .Validate(ref trip, fieldname);

        this.SummaryId.Validation("SummaryId", model, validationMessageStore)
            .NotEmpty("Guid can't be empty")
            .Validate(ref trip, fieldname);

        this.TemperatureC.Validation("TemperatureC", model, validationMessageStore)
            .GreaterThan(-61, "The minimum Temperatore is -60C")
            .LessThan(81, "The maximum temperature is 80C")
            .Validate(ref trip, fieldname);

        return !trip;
    }

}
