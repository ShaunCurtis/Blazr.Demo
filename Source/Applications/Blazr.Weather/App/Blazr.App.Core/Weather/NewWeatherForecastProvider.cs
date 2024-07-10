/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class NewWeatherForecastProvider : INewProvider<DmoWeatherForecast>
{
    public DmoWeatherForecast GetNew()
    {
        return new DmoWeatherForecast() { WeatherForecastId = new(Guid.NewGuid()), Date = DateOnly.FromDateTime(DateTime.Now) };
    }
}

