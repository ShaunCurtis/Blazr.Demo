/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed record DboInvoice
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;

    [NotMapped] public Blazr.Core.EntityState EntityState { get; init; }

    public int StateCode { get; init; } = 1;

    public Guid CustomerUid { get; init; } = Guid.Empty;

    public DateOnly InvoiceDate { get; init; } = DateOnly.FromDateTime(DateTime.MinValue);

    public string InvoiceNumber { get; init; } = "Not Set";

    public decimal InvoicePrice { get; init; }
}
