/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class InvoicePresentationServices
{
    public static void AddInvoicePresentationServices(this IServiceCollection services)
    {
        services.AddScoped<InvoiceEntityService>();
        services.AddScoped<INotificationService<InvoiceEntityService>, NotificationService<InvoiceEntityService>>();

        services.AddScoped<InvoiceAggregateManager>();

        services.AddTransient<IReadPresenter<Invoice>, ReadPresenter<Invoice>>();
        services.AddTransient<IBlazrEditPresenter<Invoice, InvoiceEditContext>, InvoiceEditPresenter>();

        services.AddScoped<IListPresenter<Invoice, InvoiceEntityService>, ListPresenter<Invoice, InvoiceEntityService>>();
        services.AddTransient<IRecordSorter<Invoice>, InvoiceSorter>();
        services.AddTransient<IRecordFilter<Invoice>, InvoiceFilter>();
        services.AddTransient<IListController<Invoice>, ListController<Invoice>>();

        services.AddTransient<IReadPresenter<InvoiceItem>, ReadPresenter<InvoiceItem>>();
        services.AddTransient<IBlazrEditPresenter<InvoiceItem, InvoiceItemEditContext>, InvoiceItemEditPresenter>();

        services.AddScoped<IListPresenter<InvoiceItem, InvoiceEntityService>, InvoiceItemListPresenter>();
        services.AddTransient<IListController<InvoiceItem>, ListController<InvoiceItem>>();
        services.AddTransient<IRecordSorter<InvoiceItem>, InvoiceItemSorter>();
        services.AddTransient<IRecordFilter<InvoiceItem>, InvoiceItemFilter>();
    }
}
