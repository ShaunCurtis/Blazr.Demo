/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components.ComponentScopedServices;

public readonly struct ComponentServiceHandle
{
    private readonly IComponentServiceProvider _componentServiceProvider;
    private readonly Guid _componentServiceKey;

    public ComponentServiceHandle(IComponentServiceProvider componentServiceProvider, Guid componentServiceKey)
    {
        _componentServiceProvider = componentServiceProvider;
        _componentServiceKey = componentServiceKey;
    }

    public TService? GetService<TService>()
       where TService : class
       => _componentServiceProvider.GetOrCreateService<TService>(_componentServiceKey);

    public object? GetService(Type service)
       => _componentServiceProvider.GetOrCreateService(_componentServiceKey, service);
}        
