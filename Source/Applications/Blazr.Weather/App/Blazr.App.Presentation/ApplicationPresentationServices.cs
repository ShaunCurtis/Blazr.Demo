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
        services.AddTransient<IFluentGridListPresenter<DmoWeatherForecast>, FluentGridPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }
}
