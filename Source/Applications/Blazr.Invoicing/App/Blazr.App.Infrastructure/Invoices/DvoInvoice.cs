/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record DvoInvoice : IKeyedEntity
{
    [Key] public Guid InvoiceID { get; init; }
    public Guid CustomerID { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }

    public object KeyValue => InvoiceID;
}
