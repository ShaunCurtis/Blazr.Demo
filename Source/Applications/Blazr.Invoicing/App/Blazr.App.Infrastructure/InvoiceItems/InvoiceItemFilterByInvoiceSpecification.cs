/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Linq.Expressions;

namespace Blazr.App.Infrastructure;

public class InvoiceItemFilterByInvoiceSpecification : PredicateSpecification<DboInvoiceItem>
{
    private InvoiceId _invoiceId = new(Guid.Empty);

    public InvoiceItemFilterByInvoiceSpecification()
    { }

    public InvoiceItemFilterByInvoiceSpecification(FilterDefinition filter)
    {
        filter.TryFromJson<InvoiceId>(out InvoiceId? _id);
        _invoiceId = _id ?? new(Guid.Empty);
    }

    public override Expression<Func<DboInvoiceItem, bool>> Expression
        => item => item.InvoiceID == _invoiceId.Value;
}
