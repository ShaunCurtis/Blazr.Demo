/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class InvoiceCompositeFactory
{
    private readonly IServiceProvider _serviceProvider;

    public InvoiceCompositeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public InvoiceComposite GetInstance(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, bool isNew = false)
    {
        return new InvoiceComposite(_serviceProvider, invoice, invoiceItems, isNew);
    }
}
