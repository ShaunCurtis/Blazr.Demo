/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public static class WeatherForecastServiceCollection
{
    public static void AddWeatherForecastServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService<WeatherForecastService>, StandardNotificationService<WeatherForecastService>>();
        services.AddScoped<IListService<DvoWeatherForecast>, StandardListService<DvoWeatherForecast, WeatherForecastService>>();
        services.AddScoped<IReadService<DvoWeatherForecast>, StandardReadService<DvoWeatherForecast, WeatherForecastService>>();
        services.AddScoped<IReadService<DboWeatherForecast>, StandardReadService<DboWeatherForecast, WeatherForecastService>>();
        services.AddScoped<IEditService<DboWeatherForecast, DeoWeatherForecast>, StandardEditService<DboWeatherForecast, DeoWeatherForecast, WeatherForecastService>>();
        services.AddScoped<WeatherForecastService>();
    }
}
