/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed class DboWeatherForecastMap : IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>
{
    public DmoWeatherForecast MapTo(DboWeatherForecast item)
        => Map(item);

    public DboWeatherForecast MapTo(DmoWeatherForecast item)
        => Map(item);

    public static DmoWeatherForecast Map(DboWeatherForecast item)
        => new()
        {
      Id = new(item.WeatherForecastID),
            Date = new(item.Date),
            Temperature = new(item.Temperature),
            Summary = item.Summary ?? "Not Defined"
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            WeatherForecastID = item.Id.Value,
            Date = item.Date.Value.ToDateTime(TimeOnly.MinValue),
            Temperature = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
