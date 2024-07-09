/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Presentation.FluentUI;
using Blazr.App.Presentation.Mud;
using Blazr.App.Presentation.Toasts;
using Blazr.App.Presentation.Vanilla;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI.Vanilla;

public static class ApplicationVanillaUIServices
{
    public static void AddAppVanillaUIServices(this IServiceCollection services)
    {
        //services.AddScoped<IAppToastService, Van>();

    }
}
