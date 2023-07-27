/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class CustomerInfrastructureServices
{
    public static void AddCustomerServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboCustomer, Customer>, DboCustomerMap>();
        services.AddScoped<ICommandHandler<Customer>, CustomerCommandHandler<InMemoryInvoiceDbContext>>();
    }
}
