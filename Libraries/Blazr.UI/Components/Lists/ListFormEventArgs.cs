/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ListFormEventArgs : EventArgs
{
    public Guid RecordId { get; set; }

    public ListFormEventArgs(Guid recordId)
        => RecordId = recordId;
}

