﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Presentation.Vanilla;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public static class ApplicationPresentationServices
{
    public static void AddAppVanillaPresentationServices(this IServiceCollection services)
    {
        AddWeatherForecastVanillaServices(services);
    }

    private static void AddWeatherForecastVanillaServices(IServiceCollection services)
    {
        services.AddTransient<IVanillaGridPresenter<DmoWeatherForecast>, VanillaGridPresenter<DmoWeatherForecast>>();
        services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<WeatherForecastEditPresenter>();
    }
}
