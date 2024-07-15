/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class WeatherForecastEditContext
{
    public DmoWeatherForecast BaseRecord { get; }

    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public WeatherForecastId Id => this.BaseRecord.WeatherForecastId;
    public bool IsDirty => this.BaseRecord != this.AsRecord;

    public DmoWeatherForecast AsRecord =>
        this.BaseRecord with
        {
            Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now),
            Summary = this.Summary,
            Temperature = new(this.Temperature)
        };

    public WeatherForecastEditContext(DmoWeatherForecast record)
    {
        this.BaseRecord = record;
        this.Load(record);
    }

    public void Load(DmoWeatherForecast record)
    {
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
    }

    public void Reset()
        => this.Load(this.BaseRecord);
}
