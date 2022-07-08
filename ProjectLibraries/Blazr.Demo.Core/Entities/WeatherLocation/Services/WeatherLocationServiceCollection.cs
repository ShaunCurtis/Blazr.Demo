/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public static class WeatherLocationServiceCollection
{
    public static void AddWeatherLocationServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService<WeatherLocationEntity>, StandardNotificationService<WeatherLocationEntity>>();
        services.AddScoped<IListService<DboWeatherLocation>, StandardListService<DboWeatherLocation, WeatherLocationEntity>>();
        services.AddScoped<IReadService<DboWeatherLocation>, StandardReadService<DboWeatherLocation, WeatherLocationEntity>>();
        services.AddScoped<IEditService<DboWeatherLocation, DeoWeatherLocation>, StandardEditService<DboWeatherLocation, DeoWeatherLocation, WeatherLocationEntity>>();
        services.AddScoped<WeatherLocationService>();
    }
}
