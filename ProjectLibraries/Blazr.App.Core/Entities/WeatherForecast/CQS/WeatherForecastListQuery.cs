/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record WeatherForecastListQuery
    : IListQuery<DvoWeatherForecast>
{

    public ListProviderRequest<DvoWeatherForecast> Request { get; private set; }

    public Guid TransactionId => Guid.NewGuid();

    public readonly Guid? WeatherLocationId;

    public WeatherForecastListQuery(Guid? weatherLocationId, ListProviderRequest<DvoWeatherForecast> request)
    {
        if (weatherLocationId is not null && weatherLocationId != Guid.Empty)
            WeatherLocationId = weatherLocationId;

        Request = request;
    }

    public WeatherForecastListQuery(ListProviderRequest<DvoWeatherForecast> request)
    {
        WeatherLocationId = Guid.Empty;
        Request = request;
    }
}
