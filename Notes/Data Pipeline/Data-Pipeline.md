#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

The primary implementation uses Entity Framework as the **Object Request Mapper**, overlayed by a thin organisational broker layer. Entity Framework implements the basic repository and unit of work patterns.

It's intent is to address persistence and retrieval in a standards based generic context with custom implementations to handle more complex and  aggregate cases.

All data within the data pipeline is *READONLY*: declared as either `record` or `readonly struct`.  

Data retrieved from a data source is a **copy** of the data within the data source.  You don't mutate the source data by changing the copy: you pass a mutated copy to the data store.

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

The ListPresenter implementations manage the in and out mappings between the request and result objects specific to the grid implementations and the pipeline objects.  In the solution there are implementations for FluentUI, MudBlazor and QuickGrid. 

The service definition for the WeatherForecast FluentUI record is:

```csharp
services.AddTransient<IFluentGridPresenter<DmoWeatherForecast>, FluentGridPresenter<DmoWeatherForecast>>();
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

The service definition for the WeatherForecast record is:

```csharp
services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
```

### Editing a Record

There is no defined generic presenter.  All presenters are record specific.

```csharp
services.AddTransient<WeatherForecastEditPresenter>();
```
