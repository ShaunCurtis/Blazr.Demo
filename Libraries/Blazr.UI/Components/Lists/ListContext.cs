/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ListContext
{
    private string sortDirectionText => SortDescending ? " Desc" : string.Empty;

    public readonly Guid Id = Guid.NewGuid();

    public event EventHandler<SortRequest>? SortingRequested;

    public event EventHandler<PagingRequest?>? PagingRequested;

    public event EventHandler<ListState>? PageUpdated;

    public event EventHandler<EventArgs>? PagingReset;

    public ListState ListState { get; private set; } = new ListState();

    public string? SortField => ListState.SortField;

    public bool SortDescending => ListState.SortDescending;

    public int PageSize => ListState.PageSize;

    public int StartIndex => ListState.StartIndex;

    public int ListTotalCount => ListState.ListTotalCount;

    public int Page => StartIndex / PageSize;

    public string SortExpression => $"{SortField}{sortDirectionText}";

    public ListContext() { }

    public void NotifyPagingRequested(object? sender, PagingRequest? request)
        => this.PagingRequested?.Invoke(sender, request);

    public void NotifySortingRequested(object? sender, SortRequest request)
        => this.SortingRequested?.Invoke(sender, request);

    public void NotifyPagingReset(object? sender)
        => this.PagingReset?.Invoke(sender, EventArgs.Empty);

    public void NotifyPageUpdated(object? sender, ListState listState)
    {
        this.ListState = listState;
        this.PageUpdated?.Invoke(sender, listState);
    }
}
