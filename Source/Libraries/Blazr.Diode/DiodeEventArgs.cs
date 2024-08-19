/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public class DiodeEventArgs : EventArgs
{
    public object? Item { get; set; }
    public DiodeState State { get; set; }

    public DiodeEventArgs(object? item, DiodeState state)
    {
        Item = item;
        State = state;
    }
}