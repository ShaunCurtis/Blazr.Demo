/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class NewInvoiceItemProvider : INewRecordProvider<DmoInvoiceItem>
{
    public InvoiceId InvoiceId { get; private set; } = InvoiceId.NewEntity;

    public void SetInvoiceContext(InvoiceId invoiceId)
        => this.InvoiceId = invoiceId;

    public DmoInvoiceItem NewRecord()
    {
        return new DmoInvoiceItem() { InvoiceId = this.InvoiceId };
    }
}