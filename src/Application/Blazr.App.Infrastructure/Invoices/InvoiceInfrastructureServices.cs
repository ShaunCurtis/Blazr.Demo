/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class InvoiceInfrastructureServices
{
    public static void AddInvoiceServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IItemRequestHandler<InvoiceAggregate>, InvoiceAggregateItemRequestHandler<InMemoryInvoiceDbContext>>();
        services.AddScoped<ICommandHandler<InvoiceAggregate>, InvoiceAggregateCommandHandler<InMemoryInvoiceDbContext>>();
        services.AddScoped<ICommandHandler<Invoice>, InvoiceCommandHandler<InMemoryInvoiceDbContext>>();
        services.AddScoped<ICommandHandler<InvoiceItem>, InvoiceItemCommandHandler<InMemoryInvoiceDbContext>>();
    }
}
