/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public class WeatherForecastSortHandler : RecordSortHandler<DboWeatherForecast>, IRecordSortHandler<DboWeatherForecast>
{
    public WeatherForecastSortHandler()
    {
        DefaultSorter = (item) => item.Date;
        DefaultSortDescending = true;
        PropertyNameMap = new Dictionary<string, string>()
            {
                {"Temperature.TemperatureC", "Temperature" },
                {"Temperature.TemperatureF", "Temperature" }
            };
    }
}
