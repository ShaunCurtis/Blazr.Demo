/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public class DboProductMap : IDboEntityMap<DboProduct, Product>
{
    public Product Map(DboProduct item)
        => new()
        {
            ProductUid = new(item.Uid),
            ProductName = item.ProductName,
            ProductCode = item.ProductCode,
            ProductUnitPrice = item.ProductUnitPrice,
            EntityState = new(StateCodes.Existing),
        };

    public DboProduct Map(Product item)
        => new()
        {
            Uid = item.Uid.Value,
            EntityState = item.EntityState,
            ProductName = item.ProductName,
            ProductCode = item.ProductCode,
            ProductUnitPrice = item.ProductUnitPrice,
        };
}
