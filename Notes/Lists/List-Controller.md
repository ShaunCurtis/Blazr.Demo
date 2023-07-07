# List Contoller

List forms always turn out far most complex than a simple interation table.  They normally incorporate one or more of the following: paging, sorting, filtering and editing.

This design uses a DataGrid component that builds the table.  It's detail is covered in another note.

List operations revolve around a `ListController` that is registered with DI as a *Transient* service.

Here's the class outline.  It holds internally the data set as a List, and exposes the list by  implementing `IEnumerable<T>`.

```csharp
public sealed class ListController<TRecord> : IEnumerable<TRecord>
    where TRecord : class, new()
{
    private ILogger<ListController<TRecord>> _logger;
    private List<TRecord>? _items = new List<TRecord>();

    private IListEventConsumer<TRecord>? _consumer;
    private IListPagerProvider? _pagerProvider;
    private IListFilterProvider? _filterProvider;

    public readonly Guid Id = Guid.NewGuid();

    public event EventHandler<EventArgs>? StateChanged;

    public readonly ListState<TRecord> ListState = new ListState<TRecord>();
    public bool IsPaging;
    public bool HasList;
    public bool HasPager;

    public ListController(ILogger<ListController<TRecord>> logger)
        => _logger = logger;

    public void RegisterPager(IListPagerProvider? pager);
    public void RegisterForEvents(IListEventConsumer<TRecord>? consumer);
    public void RegisterFilter(IListFilterProvider? filter);
    

    public void Set(ListQueryRequest<TRecord> request, ListQueryResult<TRecord> result);
    public void Reset();

    public async ValueTask NotifyPagingRequestedAsync(object? sender, PagingEventArgs request);
    public async ValueTask NotifySortingRequestedAsync(object? sender, SortEventArgs request);
    public async ValueTask NotifyFilteringRequestedAsync(object? sender, FilterEventArgs<TRecord> request);
    public void NotifyStateChanged(object? sender);

    public IEnumerator<TRecord> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator();
}
```

Lets look at how paging works.  The paging control:

Registers itself with the controller.  Only one can be registered.
 
```csharp
public void RegisterPager(IListPagerProvider? pager) => _pagerProvider = pager;
```

And can then raise a paging event with the controller by calling `NotifyPagingRequestedAsync`.  The notifier calls `PagingRequestedAsync` on the registered consumer.

```csharp
public async ValueTask NotifyPagingRequestedAsync(object? sender, PagingEventArgs request)
{
    if (sender is null || sender != _pagerProvider)
    {
        _logger.LogError($"{sender?.GetType().Name} attempted to raise a Paging Event but is not the registered Pager Provider.");
        return;
    }

    if (_consumer is not null)
        await _consumer.PagingRequestedAsync(sender, request);
}
```

`PagingEventArgs` contains a `PagingRequest` and `InitialPageSize` which provides a mechanism for passing in a default page size into the consumer if the request is null.

```csharp
public sealed class PagingEventArgs : EventArgs
{
    public PagingRequest? Request { get; set; }
    public int? InitialPageSize { get; set; }
}
```

`PagingRequest` contains the basic paging data.

```csharp
public record PagingRequest
{
    public int PageSize { get; init; } = 1000;
    public int StartIndex { get; init; } = 0;
    public int Page => StartIndex <= 0 ? 0 : StartIndex / PageSize;
}
```
