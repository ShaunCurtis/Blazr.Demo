/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public static class WeatherSummaryServices
{
    public static void AddWeatherSummaryServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService<WeatherSummaryService>, StandardNotificationService<WeatherSummaryService>>();
        services.AddScoped<IListService<DboWeatherSummary>, StandardListService<DboWeatherSummary, WeatherSummaryService>>();
        services.AddScoped<IForeignKeyService<FkWeatherSummary, WeatherSummaryService>, StandardForeignKeyService<FkWeatherSummary, WeatherSummaryService>>();
    }
}
