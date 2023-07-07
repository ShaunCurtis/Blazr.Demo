/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class CustomerSorter : RecordSorter<Customer>, IRecordSorter<Customer>
{
    protected override Expression<Func<Customer, object>> DefaultSorter { get; } = (item) => item.CustomerName ?? string.Empty;
    protected override bool DefaultSortDescending { get; } = false;
}
