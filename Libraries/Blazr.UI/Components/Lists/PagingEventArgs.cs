/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class PagingEventArgs : EventArgs
{
    public PagingState PagingOptions { get; set; }

    public PagingEventArgs(PagingState pagingOptions)
        => PagingOptions = pagingOptions;
}

