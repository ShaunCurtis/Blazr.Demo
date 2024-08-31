/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class ApplicationPresentationServices
{
    public static void AddAppServerPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<EditPresenterFactory>();

        services.AddCustomerPresentationServices();
        services.AddInvoicePresentationServices();
        services.AddInvoiceItemPresentationServices();
    }
}
