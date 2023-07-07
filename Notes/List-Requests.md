# List Requests

Generically we can define a request for a single item like this:

```csharp
ListResult GetItemsAsync(ListRequest request);
```

## The Challenges

Lists present the greatest challenges in data retrieval.

Why can't you just do:

```
var list = await dbContext.Set<TRecord>().ToListAsync();
```

Consider the following:

1. Do you really want to retrieve all the records at once.
2. What are you going to do with 2000 retrieved records?  Display them on a single page?
3. Can you gracefully handle retrieving a million records?
4. What order do you want the data set in?
5. Do you want to filter the data set?
6. Requests should be able to be passed over API calls.

## The Request

Building these requirments into the design, we have a record like this:

```
public sealed record ListQueryRequest
{
    public int StartIndex { get; init; } = 0;
    public int PageSize { get; init; } = 1000;
    public CancellationToken Cancellation { get; set; } = new();
    public IEnumerable<SortDefinition> Sorters { get; init; } = Enumerable.Empty<SortDefinition>();
    public IEnumerable<FilterDefinition> Filters { get; init; } = Enumerable.Empty<FilterDefinition>();
}
```

#### Paging

`StartIndex` and `PageSize` define the data chunk we want to retrieve.  We can apply the values as `Skip` and `Take` actions in the `IQueryable` query.

#### Sorting

A `SortDefinition` looks like this which is self explanatory.  We can use the field and sort direction to build an `Expression` in the Handler.

```csharp
public record struct SortDefinition
{
    public string SortField { get; init; } = string.Empty;
    public bool SortDescending { get; init; }

    public SortDefinition( string sortField, bool sortDescending) 
    { 
        SortField = sortField;
        SortDescending = sortDescending;
    }
}
```


#### Filtering

A `FilterDefintion` looks like this.  This is a little more complex and loosely coupled.  We are providing the handler with a string that defines the filter to apply, and a Json object.  The handler needs to lookup the filter and create and instance of the filter.  The filter knows the object type to deserialize the json object to.

Why the loose coupling?  We can't define interfaces in API calls and we can't pass generic `object` types in Json data.

```csharp
public record struct FilterDefinition
{
    public string FilterName { get; init; } = string.Empty;
    public string FilterData { get; init; } = string.Empty;

    public FilterDefinition( string filterName, string filterData) 
    { 
        FilterName = filterName;
        FilterData = filterData;
    }

    public bool TryFromJson<T>([NotNullWhen(true)] out T? value)
    {
        JsonSerializerOptions options = new() { IncludeFields = true };
        value = JsonSerializer.Deserialize<T>(this.FilterData, options);
        return value is not null;
    }

    public T? FromJson<T>()
        => JsonSerializer.Deserialize<T>(this.FilterData);

    public static FilterDefinition ToJson<T>(string name, T obj)
    {
        JsonSerializerOptions options = new() { IncludeFields = true };
        var json = JsonSerializer.Serialize<T>(obj, options);
        return new(filterName: name, filterData: json );
    }
}
```

We'll see how the handler plug this together shortly.

## The Result

The result is much simpler and can be defined using generics.  We return the requested list as a `IEnumerable` i.e. a reference to a list object that the receiver can enumerate, the total records in the query after applying filtering, and the standard status information.

```csharp
ord> : IDataResult
{
    public IEnumerable<TRecord> Items { get; init;} = Enumerable.Empty<TRecord>();  
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;
    public long TotalCount { get; init; }

    private ListQueryResult() { }

    public static ListQueryResult<TRecord> Success(IEnumerable<TRecord> Items, long totalCount, string? message = null)
        => new ListQueryResult<TRecord> {Successful=true,  Items= Items, TotalCount = totalCount, Message= message ?? string.Empty };

    public static ListQueryResult<TRecord> Failure(string message)
        => new ListQueryResult<TRecord> { Message = message};
}
```

There are two static constructors to control how a result is constructed: it either succeeded or failed.

## The Handler

The Core domain defines a *contract* interface that it uses to get items.  It doesn't care where they come from.  You may be implementing Blazor Server and calling directly into the database, or Blazor WASM and making API calls.

This is `IListRequestHandler`.  There are two implementations: one for generic handlers and one for individual object based handlers.  They both define a single `ExecuteAsync` method.

```csharp
public interface IListRequestHandler
{
    public ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class, new();
}

public interface IListRequestHandler<TRecord>
    where TRecord : class, new()
{
    public ValueTask<ListQueryResult<TRecord>> ExecuteAsync(ListQueryRequest request);
}
```

### Server Handler

The handler basic structure looks like this.  `TDbContext` defines the `DbContext` to obtain through the DbContext Factory service.   

```csharp
public sealed class ListRequestServerHandler<TDbContext>
    : IListRequestHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public ListRequestServerHandler(IDbContextFactory<TDbContext> factory, IServiceProvider serviceProvider)
    {
        _factory = factory;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class, new()
    {
         //...
    }

    private async ValueTask<ListQueryResult<TRecord>> GetItemsAsync<TRecord>(ListQueryRequest request)
    where TRecord : class, new()
    {
         //...
    }
}
```

The default server method looks like this.  It gets a *unit of work* `DbContext` from the factory, turns off tracking [this is only a query] anf gets the record through the `DbSet` in the `DbContext` using the provided `Uid`.  It returns an `ItemQueryResult` based on the result.

```
        int count = 0;
        if (request == null)
            throw new DataPipelineException($"No ListQueryRequest defined in {this.GetType().FullName}");

        var sorterProvider = _serviceProvider.GetService<IRecordSorter<TRecord>>();
        var filterProvider = _serviceProvider.GetService<IRecordFilter<TRecord>>();

        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();
        if (filterProvider is not null)
            query = filterProvider.AddFilterToQuery(request.Filters, query);

        count = query is IAsyncEnumerable<TRecord>
            ? await query.CountAsync(request.Cancellation)
            : query.Count();

        if (sorterProvider is not null)
            query = sorterProvider.AddSortToQuery(query, request.Sorters);

        if (request.PageSize > 0)
            query = query
                .Skip(request.StartIndex)
                .Take(request.PageSize);

        var list = query is IAsyncEnumerable<TRecord>
            ? await query.ToListAsync()
            : query.ToList();

        return ListQueryResult<TRecord>.Success(list, count);
```

The final method implements `IItemRequestHandler.ExecuteAsync`.  It checks to see if a specific `TRecord` implemented `IItemRequestHandler` is registered in the service container, and if so executes it instead of the default handler.

```csharp
    public async ValueTask<ListQueryResult<TRecord>> ExecuteAsync<TRecord>(ListQueryRequest request)
        where TRecord : class, new()
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

### Sorting

`IRecordSorter` defines a single method that takes in a list of `SortDefinition` objects.

```csharp
public interface IRecordSorter<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> AddSortToQuery(IQueryable<TRecord> query, IEnumerable<SortDefinition> definitions);
}
```

The generic implementation.  `AddSortToQuery` applies the provided list of `SortDefintions` to the `IQueryable` object.  A default sort can be defined in specific `TRecord` implementations.

`TryBuildSortExpression` attempts to build an `Expression` object from the definition.  You can review the code in the repository.

```csharp
public class RecordSorter<TRecord> : IRecordSorter<TRecord>
    where TRecord : class
{
    protected virtual Expression<Func<TRecord, object>>? DefaultSorter => null;
    protected virtual bool DefaultSortDescending { get; }

    public IQueryable<TRecord> AddSortToQuery(IQueryable<TRecord> query, IEnumerable<SortDefinition> definitions)
    {
        if (definitions.Count() == 0)
        {
            AddDefaultSort(query);
            return query;
        }

        foreach (var defintion in definitions)
            AddSort(query, defintion);

        return query;
    }

    protected IQueryable<TRecord> AddSort(IQueryable<TRecord> query, SortDefinition definition)
    {
        Expression<Func<TRecord, object>>? expression = null;

        if (RecordSorterFactory.TryBuildSortExpression(definition.SortField, out expression))
        {
            if (expression is not null)
                return definition.SortDescending
                ? query.OrderByDescending(expression)
                : query.OrderBy(expression);
        }

        return query;
    }

    protected IQueryable<TRecord> AddDefaultSort(IQueryable<TRecord> query)
    {
        if (this.DefaultSorter is not null)
            return this.DefaultSortDescending
            ? query.OrderByDescending(this.DefaultSorter)
            : query.OrderBy(this.DefaultSorter);

        return query;
    }
}
```

This is the `CustomerSorter`.  It inherits from `RecordSorter` and implements a default sorter on `CustomerName`.

```csharp
public class CustomerSorter : RecordSorter<Customer>, IRecordSorter<Customer>
{
    protected override Expression<Func<Customer, object>> DefaultSorter { get; } = (item) => item.CustomerName ?? string.Empty;
    protected override bool DefaultSortDescending { get; } = false;
}
```

### Filtering

`IRecordFilter` defines two methods.  `AddFilterToQuery` adds the query defined in the Specification to the query.  `GetSpecification` is implemented in specific `TRecord` implmentations and gets the Predicate delegate to apply to the query. 

```csharp
public interface IRecordFilter<TRecord>
    where TRecord : class
{
    public IQueryable<TRecord> AddFilterToQuery(IEnumerable<FilterDefinition> filters, IQueryable<TRecord> query)
    {
        foreach (var filter in filters)
        {
            var specification = GetSpecification(filter);
            if (specification != null)
                query = specification.AsQueryAble(query);
        }

        if (query is IQueryable)
            return query;

        return query.AsQueryable();
    }

    public IPredicateSpecification<TRecord>? GetSpecification(FilterDefinition filter)
        => null;
}
```

The Invoice filter implmentation looks like this.  The names are defined in an `ApplicationConstants` object.

```
public class InvoiceFilter : IRecordFilter<Invoice>
{
    public IPredicateSpecification<Invoice>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            ApplicationConstants.Invoice.FilterByCustomerUid => new InvoicesByCustomerUidSpecification(filter),
            ApplicationConstants.Invoice.FilterByInvoiceMonth => new InvoicesByMonthSpecification(filter),
            _ => null
        };
}
```

And the `InvoicesByCustomerUidSpecification` Specification.  In this case the `FilterData` is a Guid in string format that can be extracted directly by parsing.

```csharp
public class InvoicesByCustomerUidSpecification : PredicateSpecification<Invoice>
{
    private readonly Guid _customerUid;

    public InvoicesByCustomerUidSpecification(Guid customerUid)
        => _customerUid = customerUid;
    
    public InvoicesByCustomerUidSpecification(FilterDefinition filter)
        => Guid.TryParse(filter.FilterData, out _customerUid);

    public override Expression<Func<Invoice, bool>> Expression
        => invoice => invoice.CustomerUid == _customerUid;
}
```

