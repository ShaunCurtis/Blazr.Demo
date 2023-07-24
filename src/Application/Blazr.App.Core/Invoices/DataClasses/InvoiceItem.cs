/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;
public readonly record struct InvoiceItemUid(Guid Value);

public sealed record InvoiceItem : IIdentity, IStateEntity, IAggregateItem
{
    public InvoiceItemUid InvoiceItemUid { get; init; }

    public EntityState EntityState { get; init; }

    public InvoiceUid InvoiceUid { get; init; }

    public ProductUid ProductUid { get; init; }

    public string InvoiceNumber { get; init; } = "Not Set";

    public string ProductCode { get; init; } = "Not Set";

    public string ProductName { get; init; } = "Not Set";

    public int ItemQuantity { get; init; }

    public decimal ItemUnitPrice { get; init; }

    public EntityUid Uid => new(InvoiceItemUid.Value);
}

