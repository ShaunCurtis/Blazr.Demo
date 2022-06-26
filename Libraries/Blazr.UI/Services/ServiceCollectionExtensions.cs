/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.Extensions.DependencyInjection;

namespace Blazr.UI;

public static class ServiceCollectionExtensions
{
    public static void AddUiServices(this IServiceCollection services)
    {
        services.AddScoped<UiStateService, UiStateService>();
    }
}

