/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Presentation;

public interface IListController<TRecord> : IEnumerable<TRecord>
    where TRecord : class, new()
{

    public event EventHandler<EventArgs>? StateChanged;
    public bool IsPaging { get; }
    public bool HasList { get; }
    public bool HasPager {get; }
    public ListState<TRecord> ListState { get; }

    public void RegisterPager(IListPagerProvider? pager);

    public void RegisterForEvents(IListEventConsumer<TRecord>? consumer);

    public void RegisterFilter(IListFilterProvider? filter);

    public void Set(ListQueryRequest request, ListQueryResult<TRecord> result);

    public void Reset();

    public ValueTask NotifyPagingRequestedAsync(object? sender, PagingEventArgs request);

    public ValueTask NotifySortingRequestedAsync(object? sender, SortEventArgs request);

    public ValueTask NotifyFilteringRequestedAsync(object? sender, FilterEventArgs<TRecord> request);

    public void NotifyStateChanged(object? sender);
}
