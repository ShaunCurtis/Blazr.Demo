/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public sealed class SortEventArgs : EventArgs
{
    public SortRequest? Request { get; set; }

    public SortEventArgs() {}

    public SortEventArgs(SortRequest? request)
        => Request = request;
}

