/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class CustomerInfrastructureServices
{
    public static void AddMappedCustomerServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboCustomer, DmoCustomer>, DboCustomerMap>();
        services.AddScoped<IListRequestHandler<DmoCustomer>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoCustomer, DboCustomer>>();
        services.AddScoped<IItemRequestHandler<DmoCustomer, CustomerId>, MappedItemRequestServerHandler<InMemoryTestDbContext, DmoCustomer, DboCustomer, CustomerId>>();
        services.AddScoped<ICommandHandler<DmoCustomer>, MappedCommandServerHandler<InMemoryTestDbContext, DmoCustomer, DboCustomer>>();

        services.AddTransient<IRecordSortHandler<DboCustomer>, CustomerSortHandler>();
        services.AddScoped<INewRecordProvider<DmoCustomer>, NewCustomerProvider>();
    }
}
