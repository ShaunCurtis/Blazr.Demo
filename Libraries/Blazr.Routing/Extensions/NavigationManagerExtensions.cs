/// ============================================================
/// Original Code Adam Stevenson - https://github.com/SL-AdamStevenson
/// Modified By: Shaun Curtis, Cold Elm Coders
/// License:  MIT
/// Mods: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Routing;

public static class NavigationManagerExtensions
{
    public static void AddBlazrNavigationManager(this IServiceCollection services)
    {
        services.AddScoped<IBlazrNavigationManager, BlazrNavigationManager>();
    }

    public static void AddCoreNavigationManager(this IServiceCollection services)
    {
        services.AddScoped<IBlazrNavigationManager, CoreNavigationManager>();
    }
}

