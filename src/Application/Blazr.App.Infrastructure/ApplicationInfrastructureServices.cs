/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class ApplicationInfrastructureServices
{
    public static void AddAppServerInfrastructureServices(this IServiceCollection services)
        => services.AddAppServerDataServices<InMemoryInvoiceDbContext>(options
            => options.UseInMemoryDatabase($"InvoiceDatabase-{Guid.NewGuid().ToString()}"));

    public static void AddAppTestInfrastructureServices(this IServiceCollection services)
        => services.AddAppServerDataServices<InMemoryInvoiceDbContext>(options
            => options.UseInMemoryDatabase($"InvoiceDatabase-{Guid.NewGuid().ToString()}"));

    public static void AddAppServerDataServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    {
        AddAppServerInfrastructureServices<TDbContext>(services, options);
    }

    //public static void AddAppWASMDataServices(this IServiceCollection services)
    //{
    //    AddAppWASMInfraStructureServices(services);
    //    AddAppDataServices(services);
    //}

    //public static void AddAppAPIServerDataServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    //{
    //    AddAppWASMInfraStructureServices(services);
    //}

    private static void AddAppServerInfrastructureServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(options);
        services.AddScoped<IDataBroker, ServerDataBroker>();

        // Add the standard handlers
        services.AddScoped<IListRequestHandler, ListRequestServerHandler<InMemoryInvoiceDbContext>>();
        services.AddScoped<IItemRequestHandler, ItemRequestServerHandler<InMemoryInvoiceDbContext>>();
        services.AddScoped<ICommandHandler, CommandServerHandler<InMemoryInvoiceDbContext>>();

        // Add custom handlers
        //  This is a demo server thartr simply implements the base service
        //services.AddScoped<IItemRequestHandler<Customer>, CustomerRequestServerHandler<InMemoryInvoiceDbContext>>();

        services.AddInvoiceServerInfrastructureServices();
        services.AddProductServerInfrastructureServices();
        services.AddCustomerServerInfrastructureServices();
    }

    //private static void AddAppWASMInfraStructureServices(this IServiceCollection services)
    //{
    //    services.AddScoped<IDataBroker, ServerDataBroker>();

    //    services.AddScoped<IListRequestHandler, ListRequestAPIHandler>();
    //    services.AddScoped<IItemRequestHandler, ItemRequestAPIHandler>();
    //    services.AddScoped<ICommandHandler, CommandAPIHandler>();
    //}

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceDbContext>>();

        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceDbContext>(factory);
    }
}
