/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ListContext
{
    public readonly Guid Id = Guid.NewGuid();

    public event EventHandler<SortRequest>? SortingRequested;

    public event EventHandler<PagingRequest?>? PagingRequested;

    public event EventHandler<ListState>? StateChanged;

    public event EventHandler<EventArgs>? ListChanged;

    public event EventHandler<EventArgs>? PagingReset;

    public ListState ListState { get; private set; } = new ListState();

    public ListContext() { }

    public void NotifyPagingRequested(object? sender, PagingRequest? request)
        => this.PagingRequested?.Invoke(sender, request);

    public void NotifySortingRequested(object? sender, SortRequest request)
        => this.SortingRequested?.Invoke(sender, request);

    public void NotifyPagingReset(object? sender)
        => this.PagingReset?.Invoke(sender, EventArgs.Empty);

    public void NotifyStateChanged(object? sender, ListState listState)
    {
        this.ListState = listState;
        this.StateChanged?.Invoke(sender, listState);
    }

    public void NotifyListChanged(object? sender)
        => this.ListChanged?.Invoke(sender, EventArgs.Empty);
}
