/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public sealed class ListController<TRecord> : IListController<TRecord>
    where TRecord : class, new()
{
    private ILogger<IListController<TRecord>> _logger;
    private List<TRecord>? _items = new List<TRecord>();

    private IListEventConsumer<TRecord>? _consumer;
    private IListPagerProvider? _pagerProvider;
    private IListFilterProvider? _filterProvider;

    public readonly Guid Id = Guid.NewGuid();

    public event EventHandler<EventArgs>? StateChanged;
    public bool IsPaging => (this.ListState.PageSize > 0);
    public bool HasList => _items is not null;
    public bool HasPager => _pagerProvider is not null;
    public ListState<TRecord> ListState { get; } = new ListState<TRecord>();

    public ListController(ILogger<IListController<TRecord>> logger)
        => _logger = logger;

    public void RegisterPager(IListPagerProvider? pager)
        => _pagerProvider = pager;

    public void RegisterForEvents(IListEventConsumer<TRecord>? consumer)
        => _consumer = consumer;

    public void RegisterFilter(IListFilterProvider? filter)
        => _filterProvider = filter;

    public void Set(ListQueryRequest request, ListQueryResult<TRecord> result)
    {
        _items = result.Items.ToList();
        this.ListState.Set(request, result);
    }

    public void Reset()
    {
        _items = null;
        this.ListState.SetPaging(0);
    }

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

    public async ValueTask NotifySortingRequestedAsync(object? sender, SortEventArgs request)
    {
        if (_consumer is not null)
            await _consumer.SortingRequested(sender, request ?? new());
    }

    public async ValueTask NotifyFilteringRequestedAsync(object? sender, FilterEventArgs<TRecord> request)
    {
        if (sender is null || sender != _filterProvider)
        {
            _logger.LogError($"{sender?.GetType().Name} attempted to raise a Filter Event but is not the registered Filter Provider.");
            return;
        }

        if (_consumer is not null)
            await _consumer.FilteringRequested(sender, request);
    }

    public void NotifyStateChanged(object? sender)
        => this.StateChanged?.Invoke(sender, EventArgs.Empty);

    public IEnumerator<TRecord> GetEnumerator()
    {
        List<TRecord> list = _items ?? new List<TRecord>();
        foreach (var record in list)
            yield return record;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        List<TRecord> list = _items ?? new List<TRecord>();
        foreach (var record in list)
            yield return record;
    }
}
