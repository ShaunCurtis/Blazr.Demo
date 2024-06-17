using Microsoft.Extensions.DependencyInjection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class ApplicationPresentationServices
{
    public static void AddAppServerPresentationServices(this IServiceCollection services)
    {
        AddWeatherForecastServices(services);
    }

    private static void AddWeatherForecastServices(IServiceCollection services)
    {
        services.AddTransient<IListPresenter<DmoWeatherForecast>, ListPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, Guid>, ViewPresenter<DmoWeatherForecast, Guid>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }
}
