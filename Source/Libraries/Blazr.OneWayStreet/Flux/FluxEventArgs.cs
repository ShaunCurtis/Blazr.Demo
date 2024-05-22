/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Flux;

public class FluxEventArgs : EventArgs
{
    public object? Item { get; set; }
    public FluxState State { get; set; }

    public FluxEventArgs(object? item, FluxState state)
    {
        Item = item;
        State = state;
    }
}