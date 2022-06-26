/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class PagingEventArgs : EventArgs
{
    public int Page { get; set; }

    public PagingEventArgs(int page)
        => this.Page = page;
}

