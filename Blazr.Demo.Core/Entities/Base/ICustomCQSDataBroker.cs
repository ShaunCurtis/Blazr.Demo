/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public interface ICustomCQSDataBroker
{
    public ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastBySummaryListQuery query);
}
