using Microsoft.Extensions.DependencyInjection;

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
        AddCustomerServices(services);
        AddInvoiceServices(services);
        AddInvoiceItemServices(services);
    }

    private static void AddCustomerServices(IServiceCollection services)
    {
        services.AddTransient<IListPresenter<DmoCustomer>, ListPresenter<DmoCustomer>>();
        services.AddTransient<IViewPresenter<DmoCustomer, CustomerId>, ViewPresenter<DmoCustomer, CustomerId>>();
        services.AddTransient<IEditPresenter<DmoCustomer, CustomerId, CustomerEditContext>, EditPresenter<DmoCustomer, CustomerId, CustomerEditContext>>();
        //services.AddTransient<CustomerEditPresenter>();

        services.AddScoped<IGuidLookUpPresenter<CustomerLookUpItem>, GuidLookUpPresenter<CustomerLookUpItem>>();
    }

    private static void AddInvoiceServices(IServiceCollection services)
    {
        services.AddTransient<IListPresenter<DmoInvoice>, ListPresenter<DmoInvoice>>();
        services.AddTransient<IViewPresenter<DmoInvoice, InvoiceId>, ViewPresenter<DmoInvoice, InvoiceId>>();
        //services.AddTransient<InvoiceCompositePresenter>();
        services.AddScoped<InvoiceCompositePresenterFactory>();
    }

    private static void AddInvoiceItemServices(IServiceCollection services)
    {
        services.AddTransient<IListPresenter<DmoInvoiceItem>, ListPresenter<DmoInvoiceItem>>();
        services.AddTransient<IViewPresenter<DmoInvoiceItem, InvoiceItemId>, ViewPresenter<DmoInvoiceItem, InvoiceItemId>>();
    }
}
