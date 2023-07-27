/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public class DboCustomerMap : IDboEntityMap<DboCustomer, Customer>
{
    public Customer Map(DboCustomer item)
        => new()
        {
            CustomerUid = new(item.Uid),
            CustomerName = item.CustomerName,
            EntityState = new(StateCodes.Existing),
        };

    public DboCustomer Map(Customer item)
        => new()
        {
            Uid = item.Uid.Value,
            EntityState = item.EntityState,
            CustomerName = item.CustomerName,
        };
}
