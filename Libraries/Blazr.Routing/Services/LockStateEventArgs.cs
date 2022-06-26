/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Routing;

public class LockStateEventArgs : EventArgs
{
    public bool State { get; set; } = false;

    public LockStateEventArgs() { }

    public LockStateEventArgs(bool state)
        =>this.State = state;

    public static LockStateEventArgs New(bool state)
        => new LockStateEventArgs(state);
}

