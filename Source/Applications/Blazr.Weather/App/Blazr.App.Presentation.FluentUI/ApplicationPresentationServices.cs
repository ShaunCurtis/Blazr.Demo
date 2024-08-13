/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation.FluentUI;

public static class ApplicationPresentationServices
{
    public static void AddAppFluentUIPresentationServices(this IServiceCollection services)
    {
        AddWeatherForecastFluentUIServices(services);
    }

    private static void AddWeatherForecastFluentUIServices(IServiceCollection services)
    {
        services.AddTransient<IFluentGridPresenter<DmoWeatherForecast>, FluentGridPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }
}
