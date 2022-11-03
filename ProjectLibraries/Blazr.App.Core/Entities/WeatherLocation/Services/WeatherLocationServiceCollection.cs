/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class WeatherLocationServiceCollection
{
    public static void AddWeatherLocationServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService<WeatherLocationEntity>, StandardNotificationService<WeatherLocationEntity>>();
        services.AddScoped<IListService<DboWeatherLocation, WeatherLocationEntity>, StandardListService<DboWeatherLocation, WeatherLocationEntity>>();
        services.AddScoped<IReadService<DboWeatherLocation, WeatherLocationEntity>, StandardReadService<DboWeatherLocation, WeatherLocationEntity>>();
        services.AddScoped<IEditService<DboWeatherLocation, DeoWeatherLocation, WeatherLocationEntity>, StandardEditService<DboWeatherLocation, DeoWeatherLocation, WeatherLocationEntity>>();
        services.AddScoped<IContextEditService<WeatherLocationEditContext>, StandardEditContextService<DboWeatherLocation, WeatherLocationEditContext, WeatherLocationEntity>>();
        services.AddScoped<IForeignKeyService<FkWeatherLocation, WeatherLocationEntity>, StandardForeignKeyService<FkWeatherLocation, WeatherLocationEntity>>();
        services.AddScoped<IEntityService<WeatherLocationEntity>, WeatherLocationService>();
    }
}
