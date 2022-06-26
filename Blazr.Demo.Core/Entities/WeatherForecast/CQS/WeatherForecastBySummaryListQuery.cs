/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public record WeatherForecastBySummaryListQuery
    : RecordListQuery<DvoWeatherForecast>
{
    public readonly Guid? WeatherSummaryId;

    public WeatherForecastBySummaryListQuery(Guid? weatherSummaryId, ListProviderRequest request)
    { 
        WeatherSummaryId = weatherSummaryId;
        Request = request;
    }
}
