﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class CustomerPresentationServices
{
    public static void AddCustomerPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IGuidLookUpPresenter<CustomerLookUpItem>, GuidLookUpPresenter<CustomerLookUpItem>>();
    }
}
