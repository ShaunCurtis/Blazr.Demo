/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Mud.Toasts;
using Blazr.Presentation.Toasts;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI.Mud;

public static class ApplicationMudUIServices
{
    public static void AddAppMudUIServices(this IServiceCollection services)
    {
        services.AddScoped<IAppToastService, MudBlazorUIToastService>();
    }
}
