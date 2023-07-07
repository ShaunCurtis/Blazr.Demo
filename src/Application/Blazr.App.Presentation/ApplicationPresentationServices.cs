/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class ApplicationPresentationServices
{
    public static void AddAppPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IUiStateService, UiStateService>();

        services.AddCustomerPresentationServices();
        services.AddProductPresentationServices();
        services.AddInvoicePresentationServices();
    }
}
