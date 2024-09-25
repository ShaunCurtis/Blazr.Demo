/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct InvoiceItemId : IRecordId
{
    public Guid Value { get; init; }
    public object GetValueObject() => this.Value;

    public InvoiceItemId()
        => this.Value = Guid.Empty;

    public InvoiceItemId(Guid value)
        => this.Value = value;

    public static InvoiceItemId NewEntity => new(Guid.Empty);
}

public record DmoInvoiceItem
{
    public InvoiceItemId InvoiceItemId { get; init; } = InvoiceItemId.NewEntity;
    public InvoiceId InvoiceId { get; init; } = InvoiceId.NewEntity;
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
