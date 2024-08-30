using Microsoft.Extensions.DependencyInjection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class InvoiceCompositeInfrastructureServices
{
    public static void AddMappedInvoiceCompositeServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<InvoiceComposite>, InvoiceCompositeCommandHandler<InMemoryTestDbContext>>();
        services.AddScoped<IItemRequestHandler<InvoiceComposite, InvoiceId>, InvoiceCompositeItemRequestHandler<InMemoryTestDbContext>>();

        services.AddSingleton<InvoiceCompositeFactory>();
        services.AddSingleton<FluxGateDispatcher<DmoInvoice>, InvoiceDispatcher>();
        services.AddSingleton<FluxGateDispatcher<DmoInvoiceItem>, InvoiceItemDispatcher>();
    }
}
