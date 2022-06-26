/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public static class WeatherForecastServices
{
    public static void AddWeatherForecastServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService<WeatherForecastService>, StandardNotificationService<WeatherForecastService>>();
        services.AddScoped<IListService<DvoWeatherForecast>, StandardListService<DvoWeatherForecast, WeatherForecastService>>();
        services.AddScoped<ICrudService<DboWeatherForecast, DeoWeatherForecast>, StandardCrudService<DboWeatherForecast, DeoWeatherForecast, WeatherForecastService>>();
        services.AddScoped<WeatherForecastService>();
    }
}
