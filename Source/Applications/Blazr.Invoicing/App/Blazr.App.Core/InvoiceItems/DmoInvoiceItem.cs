/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record InvoiceItemId(Guid Value): IGuidKey;

public record DmoInvoiceItem : IKeyedEntity, IFluxRecord<InvoiceItemId>
{
    public InvoiceItemId InvoiceItemId { get; init; } = new(Guid.Empty);
    public InvoiceId InvoiceId { get; init; } = new(Guid.Empty);
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }

    public InvoiceItemId Id => this.InvoiceItemId;

    public object KeyValue => InvoiceItemId.Value;

}
