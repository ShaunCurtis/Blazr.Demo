namespace Blazr.App.Core;

public sealed record InvoiceId : IEntityKey
{
    public Guid Value { get; init; }

    public object KeyValue => this.Value;

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
