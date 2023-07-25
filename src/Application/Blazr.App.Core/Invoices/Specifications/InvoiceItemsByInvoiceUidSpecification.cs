/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class InvoiceItemsByInvoiceUidSpecification : PredicateSpecification<InvoiceItem>
{
    private readonly Guid _invoiceUid;

    public InvoiceItemsByInvoiceUidSpecification(Guid uid)
        => _invoiceUid = uid;

    public InvoiceItemsByInvoiceUidSpecification(FilterDefinition filter)
        => Guid.TryParse(filter.FilterData, out _invoiceUid);

    public override Expression<Func<InvoiceItem, bool>> Expression
        => item => item.InvoiceUid == new InvoiceUid( _invoiceUid);
}
