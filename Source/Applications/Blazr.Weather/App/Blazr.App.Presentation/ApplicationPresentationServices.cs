/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Presentation.FluentUI;
using Blazr.App.Presentation.MudBlazor;
using Blazr.App.Presentation.Toasts;
using Blazr.App.Presentation.Vanilla;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public static class ApplicationPresentationServices
{
    public static void AddAppFluentUIPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IAppToastService, FluentUIToastService>();

        AddWeatherForecastFluentUIServices(services);
    }
    public static void AddAppMudBlazorPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IAppToastService, VanillaUIToastService>();

        AddWeatherForecastMudBlazorServices(services);
    }
    public static void AddAppVanillaPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IAppToastService, VanillaUIToastService>();

        AddWeatherForecastVanillaServices(services);
    }

    private static void AddWeatherForecastFluentUIServices(IServiceCollection services)
    {
        services.AddTransient<IFluentGridPresenter<DmoWeatherForecast>, FluentGridPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }

    private static void AddWeatherForecastMudBlazorServices(IServiceCollection services)
    {
        services.AddTransient<IMudGridListPresenter<DmoWeatherForecast>, MudGridPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }

    private static void AddWeatherForecastVanillaServices(IServiceCollection services)
    {
        services.AddTransient<IVanillaGridPresenter<DmoWeatherForecast>, VanillaGridPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }
}
