using Microsoft.Extensions.DependencyInjection;

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
        services.AddTransient<IListPresenter<DmoCustomer>, ListPresenter<DmoCustomer>>();
        services.AddTransient<IViewPresenter<DmoCustomer, CustomerId>, ViewPresenter<DmoCustomer, CustomerId>>();
        services.AddTransient<IEditPresenter<DmoCustomer, CustomerId, CustomerEditContext>, EditPresenter<DmoCustomer, CustomerId, CustomerEditContext>>();
        //services.AddTransient<CustomerEditPresenter>();

        services.AddScoped<IGuidLookUpPresenter<CustomerLookUpItem>, GuidLookUpPresenter<CustomerLookUpItem>>();
    }
}
