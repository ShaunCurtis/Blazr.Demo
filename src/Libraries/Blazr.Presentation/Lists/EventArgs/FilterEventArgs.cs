/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public sealed class FilterEventArgs<TRecord> : EventArgs
{
    public FilterRequest<TRecord>? Request { get; set; }

    public FilterEventArgs() {}

    public FilterEventArgs(FilterRequest<TRecord>? request)
        => Request = request;
}

