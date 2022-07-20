/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record WeatherForecastListQuery
    : IFilteredListQuery<DvoWeatherForecast>
{

    public ListProviderRequest<DvoWeatherForecast> Request { get; private set; }

    public Func<DvoWeatherForecast, bool>? FilterExpression { get; private set; } = null;

    public Guid TransactionId => Guid.NewGuid();

    public readonly Guid? WeatherSummaryId;

    public WeatherForecastListQuery(Guid? weatherSummaryId, ListProviderRequest<DvoWeatherForecast> request)
    {
        if (weatherSummaryId is not null && weatherSummaryId != Guid.Empty)
        {
            WeatherSummaryId = weatherSummaryId;
            FilterExpression = (DvoWeatherForecast item) => item.WeatherSummaryId == weatherSummaryId;
        }

        Request = request;
    }

    public WeatherForecastListQuery(ListProviderRequest<DvoWeatherForecast> request, Func<DvoWeatherForecast, bool>? filter)
    {
        WeatherSummaryId = Guid.Empty;
        Request = request;
        FilterExpression = filter;
    }
}
