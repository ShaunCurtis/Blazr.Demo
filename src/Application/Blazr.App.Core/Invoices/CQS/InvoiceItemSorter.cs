/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvoiceItemSorter : RecordSorter<InvoiceItem>, IRecordSorter<InvoiceItem>
{
    protected override Expression<Func<InvoiceItem, object>> DefaultSorter => (item) => item.ItemUnitPrice;
    protected override bool DefaultSortDescending => true;
}
