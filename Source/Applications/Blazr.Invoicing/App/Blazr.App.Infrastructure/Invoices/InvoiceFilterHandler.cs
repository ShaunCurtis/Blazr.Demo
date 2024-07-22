/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class InvoiceFilterHandler : RecordFilterHandler<DvoInvoice>, IRecordFilterHandler<DvoInvoice>
{
    public override IPredicateSpecification<DvoInvoice>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            AppDictionary.Invoice.InvoiceFilterByCustomerSpecification => new InvoiceFilterByCustomerSpecification(filter),
            _ => null
        };
}
