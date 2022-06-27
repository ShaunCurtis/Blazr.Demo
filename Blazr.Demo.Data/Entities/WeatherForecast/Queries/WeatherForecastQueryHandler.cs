/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class WeatherForecastQueryHandler
    : RecordQueryHandler<DvoWeatherForecast>
{
    public WeatherForecastQueryHandler(IDbContextFactory<DbContext> factory, RecordQuery<DvoWeatherForecast> query)
        : base(factory, query)
    { }
}
