#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

The primary implementation uses Entity Framework as the **Object Request Mapper**, overlayed by a thin organisational broker layer. Entity Framework implements the basic repository and unit of work patterns.

It's intent is to address persistence and retrieval in a standards based generic context with the flexibility to handle the more compkex aggregate cases with custom implementations.

All data within the data pipeline is *READONLY*.  Data retrieved from a data source is a **copy** of the data within the data source.  You don't mutate the source data by make changes to the copy.

You change the original by passing a mutated copy of the original into the data store.

Implementing readonly objects is simple in C# 8+ using `record` value based objects with `{ get; init; }` property definitions.

The data pipeline performs two basic activities:

1. Querying for single items or lists of items.
2. Submitting conmands to change data.

## Querying

Querying can be divided into two sub-categories.

### Querying for a list of items

UI's use various types of grids and tables to display list data.  Each implement  specific request and result objects to request data and get results.

They all implement one or more of the following:

1. Paging
2. Sorting
3. Filtering 

And return:

1. A paged data set
2. The total record count in the dataset.

The pipeline implements it's own request and result objects designed to work in both server and WASM/API environments.

The ListPresenter implmentations manage the in and out mappings between the request and result objects.

The FluentUI application uses a `FluentDataGrid` to display a list of weather forecasts.  It has an associated `FluentPaginator` to control paging operations.  This plugs into the `FluentGridPresenter`. The basic structure looks like this: We provide a delegate function in the List Presenter to map  to the `ItemsProvider` parameter.  Note the clear separation of concerns.  The `FluentDataGrid` manages all the UI interactivity and the `ListPresenter`does all the data pipeline work.

```csharp
   public async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync<TGridItem>(GridItemsProviderRequest<TRecord> request)
   {
       // Build a ListRequest from the GridItemsProviderRequest 

       // Make the request into the data pipeline
       var result = await _dataBroker.ExecuteQueryAsync<TRecord>(listRequest);

       // Return a GridItemsProviderResult from the returned ListQueryResult  
       return new GridItemsProviderResult<TRecord>() { Items = result.Items.ToList(), TotalItemCount = result.TotalCount };
   }
   ```
The service definition for the WeatherForecast record is:

```csharp
services.AddTransient<IFluentGridListPresenter<DmoWeatherForecast>, FluentGridPresenter<DmoWeatherForecast>>();
```

### Querying for an item

Queries for an item from the UI use the generic `IViewPresenter<TRecord, TKey>`.

```csharp
public interface IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    public IDataResult LastDataResult { get; }
    public TRecord Item { get; }

    public Task LoadAsync(TKey id);
}
```

The standard implementation.  Definition of the key is needed for the API calls.  We can't pass generic objects through APIs.

```csharp
public class ViewPresenter<TRecord, TKey> : IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    private readonly IDataBroker _dataBroker;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public TRecord Item { get; private set; } = new();

    public ViewPresenter(IDataBroker dataBroker)
    {
        _dataBroker = dataBroker;
    }

    public async Task LoadAsync(TKey id)
    {
        var request = ItemQueryRequest<TKey>.Create(id);
        var result = await _dataBroker.ExecuteQueryAsync<TRecord, TKey>(request);
        LastDataResult = result;
        this.Item = result.Item ?? new();
    }
}
```

The service definition for the WeatherForecast record is:

```csharp
services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
```
1`