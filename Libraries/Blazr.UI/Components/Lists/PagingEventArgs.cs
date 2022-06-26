/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class PagingEventArgs : EventArgs
{
    public PagingOptions PagingOptions { get; set; }

    public PagingEventArgs(PagingOptions pagingOptions)
        => PagingOptions = pagingOptions;
}

