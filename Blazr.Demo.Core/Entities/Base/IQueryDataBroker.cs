/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public interface IQueryDataBroker
{
    public ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastListQuery query);

    public ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastBySummaryListQuery query);

    public ValueTask<FKListProviderResult> ExecuteAsync(WeatherSummaryLookupListQuery query);
}
