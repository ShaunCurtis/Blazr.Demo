/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public class DboInvoiceItemMap : IDboEntityMap<DboInvoiceItem, InvoiceItem>
{
    public InvoiceItem Map(DboInvoiceItem item)
        => new()
        {
            InvoiceItemUid = new(item.Uid),
            InvoiceUid = new(item.InvoiceUid),
            EntityState = new(StateCodes.Existing),
            ItemQuantity = item.ItemQuantity,
            ItemUnitPrice = item.ItemUnitPrice,
            ProductUid = new(item.ProductUid),
        };

    public DboInvoiceItem Map(InvoiceItem item)
        => new()
        {
            Uid = item.Uid.Value,
            EntityState = item.EntityState,
            InvoiceUid = item.InvoiceUid.Value,
            ItemQuantity = item.ItemQuantity,
            ItemUnitPrice = item.ItemUnitPrice,
            ProductUid = item.ProductUid.Value,
        };
}
