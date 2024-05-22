namespace Blazr.App.Core;

public record InvoiceId(Guid Value): IGuidKey;

public record DmoInvoice : IFluxRecord<InvoiceId>, IKeyedEntity
{
    public InvoiceId InvoiceId { get; init; } = new(Guid.Empty);
    public CustomerId CustomerId { get; init; } = new(Guid.Empty);
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateOnly Date { get; init; }

    public InvoiceId Id => this.InvoiceId;

    public object KeyValue => InvoiceId.Value;
}
