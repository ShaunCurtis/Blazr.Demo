namespace Blazr.App.Core;

public readonly record struct InvoiceId : IRecordId
{
    public Guid Value { get; init; }
    public object GetKeyObject() => this.Value;

    public InvoiceId()
        => this.Value = Guid.Empty;

    public InvoiceId(Guid value)
        => this.Value = value;

    public static InvoiceId NewEntity => new(Guid.Empty);
}

public record DmoInvoice
{
    public InvoiceId InvoiceId { get; init; } = InvoiceId.NewEntity;
    public CustomerId CustomerId { get; init; } = CustomerId.NewEntity;
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateOnly Date { get; init; }
}
