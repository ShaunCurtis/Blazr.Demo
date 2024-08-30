﻿using Microsoft.Extensions.DependencyInjection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class ApplicationInfrastructureServices
{
    public static void AddAppServerMappedInfrastructureServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<InMemoryTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        services.AddScoped<IDataBroker, DataBroker>();

        // Add the standard handlers
        services.AddScoped<IListRequestHandler, ListRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<IItemRequestHandler, ItemRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<ICommandHandler, CommandServerHandler<InMemoryTestDbContext>>();

        // Add any individual entity services
        services.AddMappedCustomerServerInfrastructureServices();
        services.AddMappedInvoiceServerInfrastructureServices();
    }

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryTestDbContext>>();

        if (factory is not null)
            TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);
    }

    public static void AddMappedCustomerServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboCustomer, DmoCustomer>, DboCustomerMap>();
        services.AddScoped<IListRequestHandler<DmoCustomer>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoCustomer, DboCustomer>>();
        services.AddScoped<IItemRequestHandler<DmoCustomer, CustomerId>, MappedItemRequestServerHandler<InMemoryTestDbContext, DmoCustomer, DboCustomer, CustomerId>>();
        services.AddScoped<ICommandHandler<DmoCustomer>, MappedCommandServerHandler<InMemoryTestDbContext, DmoCustomer, DboCustomer>>();

        services.AddTransient<IRecordSortHandler<DboCustomer>, CustomerSortHandler>();
        services.AddScoped<INewRecordProvider<DmoCustomer>, NewCustomerProvider>();
    }

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
        //services.AddTransient<InvoiceComposite>();

        services.AddSingleton<InvoiceCompositeFactory>();
        services.AddSingleton<FluxGateDispatcher<DmoInvoice>, InvoiceDispatcher>();
        services.AddSingleton<FluxGateDispatcher<DmoInvoiceItem>, InvoiceItemDispatcher>();
    }
}
