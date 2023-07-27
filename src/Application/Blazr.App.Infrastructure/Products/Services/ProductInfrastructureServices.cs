/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class ProductInfrastructureServices
{
    public static void AddProductServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IDboEntityMap<DboProduct, Product>, DboProductMap>();
        services.AddScoped<ICommandHandler<Product>, ProductCommandHandler<InMemoryInvoiceDbContext>>();
    }
}
