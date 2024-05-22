/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public class InvoiceItemSortHandler : RecordSortHandler<DboInvoiceItem>, IRecordSortHandler<DboInvoiceItem>
{
    public InvoiceItemSortHandler()
    {
        DefaultSorter = (item) => item.Description;
        DefaultSortDescending = false;
    }
}
