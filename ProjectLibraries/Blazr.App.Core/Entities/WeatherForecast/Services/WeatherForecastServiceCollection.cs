/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class WeatherForecastServiceCollection
{
    public static void AddWeatherForecastServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService<WeatherForecastEntity>, StandardNotificationService<WeatherForecastEntity>>();
        services.AddScoped<IListService<DvoWeatherForecast, WeatherForecastEntity>, StandardListService<DvoWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<IReadService<DvoWeatherForecast, WeatherForecastEntity>, StandardReadService<DvoWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<IReadService<DboWeatherForecast, WeatherForecastEntity>, StandardReadService<DboWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<IEditService<DboWeatherForecast, WeatherForecastEditContext, WeatherForecastEntity>, StandardEditService<DboWeatherForecast, WeatherForecastEditContext, WeatherForecastEntity>>();
        //services.AddScoped<IContextEditService<WeatherForecastEditContext, DboWeatherForecast>, StandardEditContextService<WeatherForecastEditContext, DboWeatherForecast, WeatherForecastEntity>>();
        services.AddScoped<IEntityService<WeatherForecastEntity>, WeatherForecastService>();
    }
}
