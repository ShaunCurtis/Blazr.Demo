/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class CustomerPresentationServices
{
    public static void AddCustomerPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IListPresenter<Customer, CustomerEntityService>, ListPresenter<Customer, CustomerEntityService>>();
        services.AddScoped<CustomerEntityService>();
        services.AddScoped<INotificationService<CustomerEntityService>, NotificationService<CustomerEntityService>>();
        services.AddScoped<IForeignKeyPresenter<CustomerFkItem, CustomerEntityService>, ForeignKeyPresenter<CustomerFkItem, CustomerEntityService>>();
        services.AddTransient<IBlazrEditPresenter<Customer, CustomerEditContext>, BlazrEditPresenter<Customer, CustomerEntityService, CustomerEditContext>>();
        services.AddTransient<IReadPresenter<Customer>, ReadPresenter<Customer>>();
        services.AddTransient<IListController<Customer>, ListController<Customer>>();
        services.AddTransient<IRecordSorter<Customer>, CustomerSorter>();
    }

}
