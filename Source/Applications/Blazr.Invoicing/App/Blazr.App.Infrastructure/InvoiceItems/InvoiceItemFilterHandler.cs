/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class InvoiceItemFilterHandler : RecordFilterHandler<DboInvoiceItem>, IRecordFilterHandler<DboInvoiceItem>
{
    public override IPredicateSpecification<DboInvoiceItem>? GetSpecification(FilterDefinition filter)
        => filter.FilterName switch
        {
            AppDictionary.InvoiceItem.InvoiceItemFilterByInvoiceSpecification => new InvoiceItemFilterByInvoiceSpecification(filter),
            _ => null
        };
}
