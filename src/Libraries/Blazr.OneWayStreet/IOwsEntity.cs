/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet
{
    public interface IOwsEntity
    {
        EntityUid EntityUid { get; }
        EntityState EntityState { get; }
    }
}
