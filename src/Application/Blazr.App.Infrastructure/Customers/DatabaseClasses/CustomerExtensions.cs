/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

internal static class CustiomerExtensions
{
    internal static DboCustomer ToDbo(this Customer item)
        => new()
        {
            Uid = item.Uid.Value,
            CustomerName = item.CustomerName,
        };

    internal static Customer FromDbo(this DboCustomer item)
        => new()
        {
            CustomerUid = new(item.Uid),
            CustomerName = item.CustomerName,
            EntityState = new(StateCodes.Existing),
        };

}
