/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.Extensions.DependencyInjection;

namespace Blazr.UI;

public static class UIServiceCollection
{
    public static void AddBlazrUIServices(this IServiceCollection services)
    {
        services.AddScoped<IUiStateService, UiStateService>();
        services.AddSingleton<ModalService>();
        services.AddSingleton<ToasterService>();
    }
}

