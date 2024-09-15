using UuidExtensions;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class NewWeatherForecastProvider : INewRecordProvider<DmoWeatherForecast>
{
    public DmoWeatherForecast NewRecord()
    {
        return new DmoWeatherForecast() { WeatherForecastId = new(Uuid7.Guid()), Date = DateOnly.FromDateTime(DateTime.Now) };
    }
}

