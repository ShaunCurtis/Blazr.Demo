/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class WeatherForecastQueryHandler
    : RecordQueryHandlerBase<DvoWeatherForecast>
{
    public WeatherForecastQueryHandler(IWeatherDbContext dbContext, RecordQuery<DvoWeatherForecast> query)
        : base(dbContext, query)
    { }
}
