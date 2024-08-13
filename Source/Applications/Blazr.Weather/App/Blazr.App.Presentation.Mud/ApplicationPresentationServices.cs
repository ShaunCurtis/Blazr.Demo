/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation.Mud;

public static class ApplicationPresentationServices
{
    public static void AddAppMudBlazorPresentationServices(this IServiceCollection services)
    {
        AddWeatherForecastMudBlazorServices(services);
    }

    private static void AddWeatherForecastMudBlazorServices(IServiceCollection services)
    {
        services.AddTransient<IMudGridListPresenter<DmoWeatherForecast>, MudGridPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }
}
