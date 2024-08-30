using Microsoft.Extensions.DependencyInjection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class InvoiceItemInfrastructureServices
{
    public static void AddMappedInvoiceItemServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboInvoiceItem, DmoInvoiceItem>, DboInvoiceItemMap>();

        services.AddScoped<IListRequestHandler<DmoInvoiceItem>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoInvoiceItem, DboInvoiceItem>>();
        services.AddScoped<IItemRequestHandler<DmoInvoiceItem, InvoiceItemId>, MappedItemRequestServerHandler<InMemoryTestDbContext, DmoInvoiceItem, DboInvoiceItem, InvoiceItemId>>();

        services.AddTransient<IRecordFilterHandler<DboInvoiceItem>, InvoiceItemFilterHandler>();
        services.AddTransient<IRecordSortHandler<DboInvoiceItem>, InvoiceItemSortHandler>();

        services.AddScoped<INewRecordProvider<DmoInvoiceItem>, NewInvoiceItemProvider>();
    }
}
