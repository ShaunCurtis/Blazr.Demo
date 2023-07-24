/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

/// <summary>
/// Value object for the State Uid
/// which is a Guid
/// </summary>
/// <param name="Value"></param>
public readonly record struct EntityUid(Guid Value)
{
    public bool IsEmpty => this.Value == Guid.Empty;

    public static EntityUid Empty => new EntityUid(Guid.Empty);
}
