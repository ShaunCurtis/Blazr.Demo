/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components.ComponentScopedServices;

public interface IComponentServiceProvider
{
    public object? GetOrCreateService(Guid componentKey, Type? serviceType);
    public TService? GetOrCreateService<TService>(Guid componentKey);
    public ValueTask ClearComponentServicesAsync(Guid componentKey);
}
