using Microsoft.Extensions.DependencyInjection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class InvoiceInfrastructureServices
{
    public static void AddMappedInvoiceServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboInvoice, DmoInvoice>, DboInvoiceMap>();
        services.AddScoped<IDboEntityMap<DvoInvoice, DmoInvoice>, DvoInvoiceMap>();
        services.AddScoped<IDboEntityMap<DboInvoiceItem, DmoInvoiceItem>, DboInvoiceItemMap>();

        services.AddScoped<IListRequestHandler<DmoInvoice>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoInvoice, DvoInvoice>>();
        services.AddScoped<IItemRequestHandler<DmoInvoice, InvoiceId>, InvoiceRequestServerHandler<InMemoryTestDbContext>>();

        services.AddScoped<IListRequestHandler<DmoInvoiceItem>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoInvoiceItem, DboInvoiceItem>>();
        services.AddScoped<IItemRequestHandler<DmoInvoiceItem, InvoiceItemId>, MappedItemRequestServerHandler<InMemoryTestDbContext, DmoInvoiceItem, DboInvoiceItem, InvoiceItemId>>();

        services.AddScoped<ICommandHandler<InvoiceComposite>, InvoiceCompositeCommandHandler<InMemoryTestDbContext>>();
        services.AddScoped<IItemRequestHandler<InvoiceComposite, InvoiceId>, InvoiceCompositeItemRequestHandler<InMemoryTestDbContext>>();

        services.AddTransient<IRecordFilterHandler<DvoInvoice>, InvoiceFilterHandler>();
        services.AddTransient<IRecordSortHandler<DvoInvoice>, InvoiceSortHandler>();

        services.AddTransient<IRecordFilterHandler<DboInvoiceItem>, InvoiceItemFilterHandler>();
        services.AddTransient<IRecordSortHandler<DboInvoiceItem>, InvoiceItemSortHandler>();

        services.AddScoped<INewRecordProvider<DmoInvoice>, NewInvoiceProvider>();
        services.AddScoped<INewRecordProvider<DmoInvoiceItem>, NewInvoiceItemProvider>();
        //services.AddTransient<InvoiceComposite>();

        services.AddSingleton<InvoiceCompositeFactory>();
        services.AddSingleton<FluxGateDispatcher<DmoInvoice>, InvoiceDispatcher>();
        services.AddSingleton<FluxGateDispatcher<DmoInvoiceItem>, InvoiceItemDispatcher>();
    }
}
