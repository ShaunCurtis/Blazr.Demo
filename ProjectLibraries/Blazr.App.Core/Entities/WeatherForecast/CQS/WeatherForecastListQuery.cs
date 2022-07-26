/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record WeatherForecastListQuery
    : ListQuery<DvoWeatherForecast>
{
    public readonly Guid? WeatherLocationId;

    public WeatherForecastListQuery()
    : base()
    => WeatherLocationId = Guid.Empty;


    public WeatherForecastListQuery(Guid? weatherLocationId, ListProviderRequest<DvoWeatherForecast> request)
        : base(request)
    {
        if (weatherLocationId is not null && weatherLocationId != Guid.Empty)
            WeatherLocationId = weatherLocationId;
    }

    public WeatherForecastListQuery(ListProviderRequest<DvoWeatherForecast> request)
        :base(request)
        => WeatherLocationId = Guid.Empty;
}
