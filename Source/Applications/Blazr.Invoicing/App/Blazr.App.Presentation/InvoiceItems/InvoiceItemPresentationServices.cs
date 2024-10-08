﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class InvoiceItemPresentationServices
{
    public static void AddInvoiceItemPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<InvoiceItemEditPresenterFactory>();
    }
}
