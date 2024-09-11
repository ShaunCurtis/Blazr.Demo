# Presenters

UI components and forms should only contain UI logic.  *Presenters* are the *Presentation* layer objects of the *Core* domain that hold and manage the data.

A generic View/Display presenter can be defined in a `IViewPresenter` interface like this.

1. `TKey` defines the identity key: it makes querying the data source simpler.
2. `Item` is the item to display.
3. `LastDataResult` is the data result returned when the `Item` was retrieved from the data pipeline.

```csharp
public interface IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    public IDataResult LastDataResult { get; }
    public TRecord Item { get; }
}
```

The implementation uses the registered `IDataBroker` to get the data from the data pipeline.

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

Constructing a populated `ViewPresenter` is a two step process:
1. Newing up a object instance
2. Getting the data object

That makes defining an `IViewPresenter` in DI messy.  The object instnce you get back isn't usable until you've called `LoadAsync`.

The solution is to use a DI defined *Factory*.

Note the constructor and load methods are internal.  They can only be called by the factory class defined within the assembly.

In the solution `PresenterFactory` is the single factory for all generic presenters.  This is the `CreateViewPresenterAsync`:  

```csharp
public async ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
    where TRecord : class, new()
    where TIdentity : IEntityKey
{
    IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();

    var presenter = new ViewPresenter<TRecord, TIdentity>(dataBroker);
    await presenter.LoadAsync(id);

    return presenter;
}
```
The factory is a *Scoped* Service and defines the `IServiceProvider` in it's constructor.  Each factory method:

1. Gets the services it needs using `GetRequiredService<T>` and creates an instance of the presenter.
2. Calls `LoadAsync` to populate the data.

## Implementation

The only service you need to define in DI is:

```csharp
services.AddScoped<IPresenterFactory, PresenterFactory>();
```

The `CustomerViewForm` injects the factory from DI:  

```csharp
@inject PresenterFactory PresenterFactory
```

And then uses the factory to get an instance of `IViewPresenter` in `SetParametersAsync`.  This ensures the `IViewPresenter` is fully loaded before any rendering takes place.

You can run the code in `OnInitializedAsync`.  If you do you will need to handle a `null` reference for `IViewPresenter` in the first render.

```csharp
private IViewPresenter<DmoCustomer, CustomerId> Presenter = default!;
private bool _isNotInitialized = true;


public override async Task SetParametersAsync(ParameterView parameters)
{
    parameters.SetParameterProperties(this);

    // Get the presenter
    if (_isNotInitialized)
    {
        this.Presenter = await this.PresenterFactory.CreateViewPresenterAsync<DmoCustomer, CustomerId>(this.Content);
        _isNotInitialized = false;
    }

    await base.SetParametersAsync(ParameterView.Empty);
}
```
