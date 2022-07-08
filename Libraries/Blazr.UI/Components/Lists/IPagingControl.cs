/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public interface IPagingControl : IDisposable
{
    [Parameter] public int PageSize { get; set; }

    [Parameter] public int BlockSize { get; set; }

    [Parameter] public Func<PagingState, ValueTask<PagingState>>? PagingProvider { get; set; }

    [Parameter] public bool ShowPageOf { get; set; }

    public void NotifyListChangedAsync();
}
