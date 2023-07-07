/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public sealed class PagingEventArgs : EventArgs
{
    public PagingRequest? Request { get; set; }

    public int? InitialPageSize { get; set; }

    public PagingEventArgs() { }

    public PagingEventArgs(PagingRequest? request)
        => Request = request;
}

