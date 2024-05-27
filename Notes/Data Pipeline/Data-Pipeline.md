#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

The primary implementation uses Entity Framework as the **Object Request Mapper**, overlayed by a thin organisational broker layer. Entity Framework implements the basic repository and unit of work patterns: there's no need for another fat layer. 

It's intent is to address persistence and retrieval in a standards based generic context with the flexibility to handle the more compkex aggregate cases with custom implementations.

All data within the data pipeline is *READONLY*.  When you retrieve data from a data source it's a **copy** of the data within the data source.  It's not a pointer to the source data that you can mutate as you wish: it's read only.

You change the original by passing a mutated copy of the original into the data store.

Implementing readonly objects is simple in C# 8+ using `record` value based objects with `{ get; init; }` property definitions.

We can divide data pipeline activity into two basic activities:

1. Querying for single items or lists of items.
2. Submitting conmands to change data.

## Querying

Querying falls into two categories.

### Querying for a list of items

The application uses a `FluentDataGrid` to display a list of weather forecasts.  It has an associated `FluentPaginator` to control paging operations. We provide a delegate function in the List Presenter to map  to the `ItemsProvider` parameter.  Note the clear separation of concerns.  The `FluentDataGrid` manages all the UI interactivity and the `ListPresenter`does all the data pipeline work.

```csharp
 GetItemsAsync<TGridItem>(GridItemsProviderRequest<TRecord> request);
```

 `GetItemsAsync` maps the `GridItemsProviderRequest` [specific to the `FluentDataGrid`] to a data pipeline `ListQueryRequest` and then passes the request to the DI registered `IDataBroker`.

```csharp
 var result = await _dataBroker.ExecuteQueryAsync<TRecord>(listRequest);
 ```

In the server registered application this passes the request to the `IListRequestHandler` DI registered handler.

 ```csharp
public ValueTask<ListQueryResult<TRecord>> ExecuteQueryAsync<TRecord>(ListQueryRequest request) where TRecord : class
        => _listRequestHandler.ExecuteAsync<TRecord>(request);
```

This checks DI to see if there's a specific handler registered for `TRecord` [in our case `DmoWeatherForecast`].  If there is, it executes the specific handler, if not the generic handler handles the request.

```csharp
    public async ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class
    {
        IListRequestHandler<TRecord>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<IListRequestHandler<TRecord>>();

        // If we get one then one is registered in DI and we execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If there's no custom handler registered we run the base handler
        return await this.GetItemsAsync<TRecord>(request);
    }
```

We have defined a custom handler:

```csharp
services.AddScoped<IListRequestHandler<DmoWeatherForecast>, MappedListRequestServerHandler<InMemoryTestDbContext, DmoWeatherForecast, DboWeatherForecast>>();
```

I'll not show the detail here, but it builds an `IQueryable` query for `DboWeatherForecast` executees the query on the data store, maps the returned `DboWeatherForecast` objects to a `DmoWeatherForecast` list and returns a `ListQueryResult<DmoWeatherForecast>`.  This passes back up to the Presenter which maps the `ListQueryResult<DmoWeatherForecast>` to a `GridItemsProviderResult<DmoWeatherForecast>` which is returned to the `FluentDataGrid`.

### Querying for an item

Queries for an item from the UI use the generic `IViewPresenter<TRecord>`.

```csharp
public interface IViewPresenter<TRecord>
    where TRecord : class, new()
{
    public IDataResult LastDataResult { get; }
    public TRecord Item { get; }

    public Task LoadAsync(object id);
}
```

It's the responsibility of the implementation to handle the object type.  The generic implementation expects a `IGuidKey`, but will pass the object if it doesn't get one.

```csharp
    public async Task LoadAsync(object id)
    {
        // Get the actual value of the Id type
        if (id is IGuidKey entity)
            id = entity.Value;

        var request = ItemQueryRequest.Create(id);
        var result = await _dataBroker.ExecuteQueryAsync<TRecord>(request);
        LastDataResult = result;
        this.Item = result.Item ?? new();
    }
```
