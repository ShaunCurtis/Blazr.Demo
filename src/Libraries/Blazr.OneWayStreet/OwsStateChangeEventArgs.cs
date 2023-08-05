/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet;

public class OwsStateChangeEventArgs : EventArgs
{
    public object Entity { get; init; }
    public EntityUid EntityUid { get; init; }

    public OwsStateChangeEventArgs(EntityUid entityUid, object entity)
    {
        Entity = entity;
        EntityUid = entityUid;
    }
}
