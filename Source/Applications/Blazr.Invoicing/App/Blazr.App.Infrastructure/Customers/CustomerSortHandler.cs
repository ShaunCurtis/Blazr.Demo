/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public class CustomerSortHandler : RecordSortHandler<DboCustomer>, IRecordSortHandler<DboCustomer>
{
    public CustomerSortHandler()
    {
        DefaultSorter = (item) => item.CustomerName;
        DefaultSortDescending = false;
    }
}
