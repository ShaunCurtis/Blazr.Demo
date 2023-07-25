/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct CustomerUid(Guid Value);

public sealed record Customer : IEntity, IStateEntity, ICommandEntity
{
    public CustomerUid CustomerUid { get; init; }

    public EntityState EntityState { get; init; }

    public string CustomerName { get; init; } = "Not Set";

    public EntityUid Uid => new(CustomerUid.Value);
}
