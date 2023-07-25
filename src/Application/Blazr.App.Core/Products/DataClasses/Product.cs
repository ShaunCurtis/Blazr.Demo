/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct ProductUid(Guid Value);

public sealed record Product : IEntity, IStateEntity, ICommandEntity
{
    public ProductUid ProductUid { get; init; }

    public EntityState EntityState { get; init; }

    public string ProductCode { get; init; } = "Not Set";

    public string ProductName { get; init; } = "Not Set";

    public decimal ProductUnitPrice { get; init; } = 0;

    public EntityUid Uid => new(ProductUid.Value);
}
