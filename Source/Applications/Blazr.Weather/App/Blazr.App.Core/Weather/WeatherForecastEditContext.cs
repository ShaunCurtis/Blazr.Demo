/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class WeatherForecastEditContext
{
    private DmoWeatherForecast _baseRecord;

    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public WeatherForecastId Id => _baseRecord.WeatherForecastId;
    public bool IsSummaryClean => Summary is not null ? Summary.Equals(_baseRecord.Summary) : true;
    public bool IsDirty => _baseRecord != this.AsRecord;

    public DmoWeatherForecast AsRecord =>
        _baseRecord with
        {
            Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now),
            Summary = this.Summary,
            Temperature = new(this.Temperature)
        };

    public WeatherForecastEditContext(DmoWeatherForecast record)
    {
        _baseRecord = record;
        this.Load(record);
    }

    public void Load(DmoWeatherForecast record)
    {
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
    }

    public void Reset()
        => this.Load(_baseRecord);
}
