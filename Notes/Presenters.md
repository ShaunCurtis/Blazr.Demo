# Presenters

Presenters are the front end interfaces into the Core domain.  They provide and manage data for components and form components.

As a demonstration a generic View/Display presenter can be defined in a `IViewPresenter` like this.

`TKey` defined the identity key to make querying the data source simpler.

```csharp
public interface IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    public IDataResult LastDataResult { get; }
    public TRecord Item { get; }
}
```

And the implementation:

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

Notice that the constructor and load methods are internal.

Initializing objects where populating the object data is asynchronous is a coding conundrum without a perfect solution.  This is further 

Using a *Fsctory* and restricting access to the constructor and load methods is one solution.

In the solution `PresenterFactory` is the single factory for all generic presenters.  This is the `CreateViewPresenterAsync`:  

```csharp
    public async ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
        where TRecord : class, new()
        where TIdentity : IEntityKey
    {
        IDataBroker dataBroker = _serviceProvider.GetRequiredService<IDataBroker>();
        IToastService toastService = _serviceProvider.GetRequiredService<IToastService>();

        var presenter = new ViewPresenter<TRecord, TIdentity>(dataBroker);
        await presenter.LoadAsync(id);

        return presenter;
    }
```

## Implementation

So, the `CustomerViewForm` injects the factory from DI:  

```csharp
@inject PresenterFactory PresenterFactory
```

And then use the factory to get an instance of `IViewPresenter`.

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

Note the `SetParametersAsync` pattern.  Code in `SetParametersAsync` [before the base call] will complete before the first render.