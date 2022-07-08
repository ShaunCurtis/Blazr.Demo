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
        services.AddScoped<INotificationService<WeatherForecastEntity>, StandardNotificationService<WeatherForecastEntity>>();
        services.AddScoped<IListService<DvoWeatherForecast>, StandardListService<DvoWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<IReadService<DvoWeatherForecast>, StandardReadService<DvoWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<IReadService<DboWeatherForecast>, StandardReadService<DboWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<IEditService<DboWeatherForecast, DeoWeatherForecast>, StandardEditService<DboWeatherForecast, DeoWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<WeatherForecastService>();
    }
}
