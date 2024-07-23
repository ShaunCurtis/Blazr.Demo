/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Bootstrap.Toasts;
using Blazr.Presentation.Toasts;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI.Vanilla;

public static class ApplicationVanillaUIServices
{
    public static void AddAppVanillaUIServices(this IServiceCollection services)
    {
        services.AddScoped<IAppToastService, BootstrapUIToastService>();
    }
}
