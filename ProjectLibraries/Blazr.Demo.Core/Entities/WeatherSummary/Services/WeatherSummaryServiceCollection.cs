/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public static class WeatherSummaryServiceCollection
{
    public static void AddWeatherSummaryServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService<WeatherSummaryEntity>, StandardNotificationService<WeatherSummaryEntity>>();
        services.AddScoped<IListService<DboWeatherSummary>, StandardListService<DboWeatherSummary, WeatherSummaryEntity>>();
        services.AddScoped<IReadService<DboWeatherSummary>, StandardReadService<DboWeatherSummary, WeatherSummaryEntity>>();
        services.AddScoped<IForeignKeyService<FkWeatherSummary, WeatherSummaryEntity>, StandardForeignKeyService<FkWeatherSummary, WeatherSummaryEntity>>();
        services.AddScoped<WeatherSummaryService>();
    }
}
