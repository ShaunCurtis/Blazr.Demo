/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class EditStateEventArgs : EventArgs
{
    public bool IsDirty { get; set; }

    public static EditStateEventArgs NewArgs(bool dirtyState)
        => new EditStateEventArgs { IsDirty = dirtyState };
}

