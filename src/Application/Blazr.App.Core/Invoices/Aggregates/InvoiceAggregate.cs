/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class InvoiceAggregate : AggregateBase<Invoice, InvoiceItem>
{
    public InvoiceAggregate() : base() { }

    public InvoiceAggregate(Invoice root, IEnumerable<InvoiceItem>? items = null) : base(root, items) { }

    protected override void NotifyUpdated()
    {
        decimal totalcost = 0;
        foreach (var item in this.LiveItems)
        {
            totalcost = totalcost + (item.ItemUnitPrice * item.ItemQuantity);
        }
        if (this.Root.InvoicePrice != totalcost)
        {
            var root = Root with { InvoicePrice = totalcost };
            this.UpdateRoot(root);
        }
    }
}
