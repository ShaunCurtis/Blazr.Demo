/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public record WeatherForecastListQuery
    :RecordListQuery<DvoWeatherForecast>
{
    public WeatherForecastListQuery(ListProviderRequest request)
        : base(request)
    { 
        Request = request;
    }
}
