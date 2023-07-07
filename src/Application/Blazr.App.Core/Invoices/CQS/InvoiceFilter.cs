/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvoiceFilter : IRecordFilter<Invoice>
{
    public IPredicateSpecification<Invoice>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            ApplicationConstants.Invoice.FilterByCustomerUid => new InvoicesByCustomerUidSpecification(filter),
            ApplicationConstants.Invoice.FilterByInvoiceMonth => new InvoicesByMonthSpecification(filter),
            _ => null
        };
}
