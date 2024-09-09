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
        services.AddScoped<IPresenterFactory, PresenterFactory>();
        services.AddScoped<FluentUIPresenterFactory>();
    }
}
