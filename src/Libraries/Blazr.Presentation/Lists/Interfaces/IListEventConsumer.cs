/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public interface IListEventConsumer<TRecord>
    where TRecord : class, new()
{
    public ValueTask PagingRequestedAsync(object? sender, PagingEventArgs request);

    public ValueTask SortingRequested(object? sender, SortEventArgs request);

    public ValueTask FilteringRequested(object? sender, FilterEventArgs<TRecord> request);

}
