# The Async Service Conundrum

Coding an object that requires and async constructor has always been a challenge.  There are solutions, but none have a warm cosy feeling.

Let's consider the use case for a Blazor presentation object.  It's purpose is to hold and manage a data object required by a component.

To keep it simple consider the View/Display Presenter.

It has two properties: an instance of `TRecord` and the data result from trying to retrieve the instance of `TRecord`. 

```csharp
public interface IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    public TRecord Item { get; }
    public IDataResult LastDataResult { get; }
}
```

The implementation requires the configured `IDataBroker` obtained though DI.

```csharp
public class ViewPresenter<TRecord, TKey> : IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    private readonly IDataBroker _dataBroker;
    public TRecord Item { get; private set; } = new();
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();

    public ViewPresenter(IDataBroker dataBroker)
    {
        _dataBroker = dataBroker;
    }

    public async ValueTask LoadAsync(TKey id)
    {
        var request = ItemQueryRequest<TKey>.Create(id);
        var result = await _dataBroker.ExecuteQueryAsync<TRecord, TKey>(request);
        LastDataResult = result;
        this.Item = result.Item ?? new();
    }
}
```

Getting a usable object is a two step process:

1. Get the object from `Dependancy Injection`.
2. Call `LoadAsync` passing in the Id.

A smelly object: far from ideal.

One solution is to use a Factory.

First, set the constructor and `LoadAsync` to `internal`, restricting access to objects within the same namespace.  The presenter now can't be used in DI or constructed in a component.

```csharp
public class ViewPresenter<TRecord, TKey> : IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    private readonly IDataBroker _dataBroker;
    public TRecord Item { get; private set; } = new();
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();

    internal ViewPresenter(IDataBroker dataBroker)
    {
        _dataBroker = dataBroker;
    }

    internal async ValueTask LoadAsync(TKey id)
    {
        var request = ItemQueryRequest<TKey>.Create(id);
        var result = await _dataBroker.ExecuteQueryAsync<TRecord, TKey>(request);
        LastDataResult = result;
        this.Item = result.Item ?? new();
    }
}
```

Define a factory class to get `IViewPresenter` instances.

```csharp
public class PresenterFactory
{
    private IServiceProvider _serviceProvider;

    public PresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
        where TRecord : class, new()
        where TIdentity : IEntityKey
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();

        var presenter = new ViewPresenter<TRecord, TIdentity>(dataBroker);
        await presenter.LoadAsync(id);

        return presenter;
    }
}
```
This gets the `IServiceProvider` instance which can then be used tp get any services defined in DI.

There are two principle methods we use:

1. `GetRequirdService(Type requiredServiceType)` and `GetRequiredService<T>()` attempt to get the specified service.  They generate exceptions if the service type is not defined in DI. 
1. `GetService(Type requiredServiceType)` and `GetService<T>` returns the requested service or `null` if the service is not defined in DI.

`Getx<T>()` returns an object of type `T`, while GetX() returns an generic object which needs to be cast.

There are also the keyed service versions that are implemented in the same way.

All of these methods return service objects managed by the service container.
