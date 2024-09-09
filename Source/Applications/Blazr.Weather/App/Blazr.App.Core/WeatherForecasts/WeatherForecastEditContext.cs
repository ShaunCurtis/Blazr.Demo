/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class WeatherForecastEditContext : IRecordEditContext<DmoWeatherForecast>
{
    public DmoWeatherForecast BaseRecord { get; private set; }
    public bool IsDirty => this.BaseRecord != this.AsRecord;

    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public DmoWeatherForecast AsRecord =>
        this.BaseRecord with
        {
            Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now),
            Summary = this.Summary,
            Temperature = new(this.Temperature)
        };

    public WeatherForecastEditContext()
    {
        this.BaseRecord = new DmoWeatherForecast();
        this.Load(this.BaseRecord);
    }

    public WeatherForecastEditContext(DmoWeatherForecast record)
    {
        this.BaseRecord = record;
        this.Load(record);
    }

    public IDataResult Load(DmoWeatherForecast record)
    {
        var alreadyLoaded = this.BaseRecord.WeatherForecastId != WeatherForecastId.NewEntity;

        if (alreadyLoaded)
            return DataResult.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
        return DataResult.Success();
    }
}
