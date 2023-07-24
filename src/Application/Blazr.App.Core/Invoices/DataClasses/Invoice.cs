/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct InvoiceUid(Guid Value);

public sealed record Invoice : IIdentity, IStateEntity, IAggregateItem
{
    public InvoiceUid InvoiceUid { get; init; }

    public EntityState EntityState { get; init; }

    public CustomerUid CustomerUid { get; init; }

    public string CustomerName { get; init; } = "Not Set";

    public DateOnly InvoiceDate { get; init; } = DateOnly.FromDateTime(DateTime.MinValue);

    public string InvoiceNumber { get; init; } = "Not Set";

    public decimal InvoicePrice { get; init; }

    public EntityUid Uid => new(InvoiceUid.Value);
}
