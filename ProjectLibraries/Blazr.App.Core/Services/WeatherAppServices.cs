/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class WeatherAppServices
{
    public static void AddWeatherServices(this IServiceCollection services)
    {
        services.AddWeatherForecastServices();
        services.AddWeatherSummaryServices();
        services.AddWeatherLocationServices();
        services.AddUserServices();
        services.AddAppAuthServerServices();
    }
}
