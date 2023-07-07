/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvoiceItemFilter : IRecordFilter<InvoiceItem>
{
    public IPredicateSpecification<InvoiceItem>? GetSpecification(FilterDefinition filter)
    => filter.FilterName switch
    {
        ApplicationConstants.InvoiceItem.FilterByInvoiceUid => new InvoiceItemsByInvoiceUidSpecification(filter),
        ApplicationConstants.InvoiceItem.FilterByProductUid => new InvoiceItemsByProductUidSpecification(filter),
        _ => null
    };
}
