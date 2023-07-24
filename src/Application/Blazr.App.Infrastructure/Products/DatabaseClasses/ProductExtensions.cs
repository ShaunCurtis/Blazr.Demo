/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

internal static class ProductExtensions
{
    internal static DboProduct ToDbo(this Product item)
        => new()
        {
            Uid = item.Uid.Value,
            ProductName = item.ProductName,
             ProductCode = item.ProductCode,
            ProductUnitPrice = item.ProductUnitPrice,
        };

    internal static Product FromDbo(this DboProduct item)
        => new()
        {
            ProductUid = new(item.Uid),
            ProductName = item.ProductName,
            ProductCode = item.ProductCode,
            ProductUnitPrice = item.ProductUnitPrice,
            EntityState = new(StateCodes.Existing),
        };
}
