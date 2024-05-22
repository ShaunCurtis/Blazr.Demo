/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class InvoiceFilterHandler : RecordFilterHandler<DboInvoice>, IRecordFilterHandler<DboInvoice>
{
    public override IPredicateSpecification<DboInvoice>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            AppDictionary.Invoice.InvoiceFilterByCustomerSpecification => new InvoiceFilterByCustomerSpecification(filter),
            _ => null
        };
}
