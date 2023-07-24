/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

internal static class InvoiceExtensions
{
    internal static DboInvoice ToDbo(this Invoice item)
        => new()
        {
            Uid = item.Uid.Value,
            StateCode = item.EntityState.StateCode.Value,
            InvoiceDate = item.InvoiceDate,
            InvoiceNumber = item.InvoiceNumber,
            InvoicePrice = item.InvoicePrice,
            CustomerUid = item.CustomerUid.Value,
        };

    internal static Invoice FromDbo(this DboInvoice item)
    => new()
    {
        InvoiceUid = new(item.Uid),
        EntityState = new(StateCodes.GetStateCode(item.StateCode)),
        InvoiceDate = item.InvoiceDate,
        InvoiceNumber = item.InvoiceNumber,
        InvoicePrice = item.InvoicePrice,
        CustomerUid = new(item.CustomerUid),
    };

    internal static DboInvoiceItem ToDbo(this InvoiceItem item)
    => new()
    {
        Uid = item.Uid.Value,
        InvoiceUid = item.InvoiceUid.Value,
        ItemQuantity = item.ItemQuantity,
        ItemUnitPrice = item.ItemUnitPrice,
        ProductUid = item.ProductUid.Value,
    };

    internal static InvoiceItem FromDbo(this DboInvoiceItem item)
    => new()
    {
        InvoiceItemUid = new(item.Uid),
        InvoiceUid = new(item.InvoiceUid),
        EntityState = new(StateCodes.Existing),
        ItemQuantity = item.ItemQuantity,
        ItemUnitPrice = item.ItemUnitPrice,
        ProductUid = new(item.ProductUid),
    };

}
