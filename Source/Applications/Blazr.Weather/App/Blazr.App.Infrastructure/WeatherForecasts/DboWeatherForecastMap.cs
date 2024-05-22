/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class DboWeatherForecastMap : IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>
{
    public DmoWeatherForecast MapTo(DboWeatherForecast item)
        => Map(item);

    public DboWeatherForecast MapTo(DmoWeatherForecast item)
        => Map(item);

    public static DmoWeatherForecast Map(DboWeatherForecast item)
        => new()
        {
            WeatherForecastId = new(item.WeatherForecastID),
            Date = DateOnly.FromDateTime(item.Date),
            Temperature = new(item.Temperature),
            Summary = item.Summary
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            WeatherForecastID = item.WeatherForecastId.Value,
            Date = item.Date.ToDateTime(TimeOnly.MinValue),
            Temperature = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
