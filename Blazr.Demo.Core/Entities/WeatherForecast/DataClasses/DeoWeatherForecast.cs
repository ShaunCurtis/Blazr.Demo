/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public class DeoWeatherForecast : IEditRecord<DboWeatherForecast>
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

    public DeoWeatherForecast() { }

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
}
