/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvoiceSorter : RecordSorter<Invoice>, IRecordSorter<Invoice>
{
    protected override Expression<Func<Invoice, object>>? DefaultSorter => (item) => item.InvoiceDate;
    protected override bool DefaultSortDescending => true;
}
