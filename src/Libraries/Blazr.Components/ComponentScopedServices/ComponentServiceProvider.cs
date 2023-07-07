/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components.ComponentScopedServices;

public record ComponentService(Guid ComponentId, Type ServiceType, object ServiceInstance);

public class ComponentServiceProvider : IComponentServiceProvider, IDisposable, IAsyncDisposable
{
    private IServiceProvider _serviceProvider;
    private List<ComponentService> _componentServices = new List<ComponentService>();
    private bool asyncdisposedValue;
    private bool disposedValue;
    public readonly Guid InstanceId = Guid.NewGuid();

    public ComponentServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? GetOrCreateService(Guid componentKey, Type? serviceType)
        => getOrCreateService(componentKey, serviceType);

    public TService? GetOrCreateService<TService>(Guid componentKey)
    {
        var service = this.getOrCreateService(componentKey, typeof(TService));
        return (TService?)service;
    }

    public ValueTask ClearComponentServicesAsync(Guid componentKey)
        => removeServicesAsync(componentKey);


    private object? getOrCreateService(Guid componentKey, Type? serviceType)
    {
        if (serviceType is null || componentKey == Guid.Empty)
            return null;

        // Try getting the service from the collection
        if (this.tryFindComponentService(componentKey, serviceType, out ComponentService? service))
            return service.ServiceInstance;

        // Try creating the service
        if (!this.tryCreateService(serviceType, out object? newService))
            this.tryCreateInterfaceService(serviceType, out newService);

        if (newService is null)
            return null;

        _componentServices.Add(new ComponentService(componentKey, serviceType, newService));

        return newService;
    }

    private bool tryCreateService(Type serviceType, [NotNullWhen(true)] out object? service)
    {

        service = null;
        try
        {
            service = ActivatorUtilities.CreateInstance(_serviceProvider, serviceType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool tryCreateInterfaceService(Type serviceType, [NotNullWhen(true)] out object? service)
    {
        service = null;
        var concreteService = _serviceProvider.GetService(serviceType);
        if (concreteService is null)
            return false;

        var concreteInterfaceType = concreteService.GetType();

        try
        {
            service = ActivatorUtilities.CreateInstance(_serviceProvider, concreteInterfaceType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async ValueTask removeServicesAsync(Guid componentKey)
    {
        foreach(var componentService in _componentServices.Where(item => item.ComponentId == componentKey))
        {
            if (componentService.ServiceInstance is IDisposable disposable)
                disposable.Dispose();

            if (componentService.ServiceInstance is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();

            _componentServices.Remove(componentService);
        }
    }

    private bool tryFindComponentService(Guid componentId, Type serviceType, [NotNullWhenAttribute(true)] out ComponentService? result)
    {
        result = _componentServices.SingleOrDefault(item => item.ComponentId == componentId && item.ServiceType == serviceType);
        if (result is default(ComponentService))
            return false;

        return true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue || !disposing)
        {
            disposedValue = true;
            return;
        }

        foreach (var componentService in _componentServices)
        {
            if (componentService.ServiceInstance is IDisposable disposable)
                disposable.Dispose();
        }

        disposedValue = true;
    }

    protected async ValueTask DisposeAsync(bool disposing)
    {
        if (asyncdisposedValue || !disposing)
        {
            asyncdisposedValue = true;
            return;
        }

        foreach (var componentService in _componentServices)
        {
            if (componentService.ServiceInstance is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
        }

        asyncdisposedValue = true;
    }
}
