/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.FluentUI.Toasts;
using Blazr.Presentation.Toasts;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI.FluentUI;

public static class ApplicationFluentUIServices
{
    public static void AddAppFluentUIServices(this IServiceCollection services)
    {
        services.AddScoped<IAppToastService, FluentUIToastService>();
    }
}
