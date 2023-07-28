/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed record DboInvoiceItem 
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;

    [NotMapped] public Blazr.Core.EntityState EntityState { get; init; }

    public Guid InvoiceUid { get; init; } = Guid.Empty;

    public Guid ProductUid { get; init; } = Guid.Empty;

    public int ItemQuantity { get; init; }

    public decimal ItemUnitPrice { get; init; }
}

