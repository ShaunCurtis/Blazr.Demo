/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

internal static class InvoiceViewExtensions
{
    internal static DboInvoice ToDbo(this Invoice item)
        => new()
        {
            Uid = item.Uid,
            StateCode = item.StateCode,
            InvoiceDate= item.InvoiceDate,
            InvoiceNumber= item.InvoiceNumber,
            InvoicePrice= item.InvoicePrice,
            CustomerUid= item.CustomerUid,
        };

    internal static DboInvoiceItem ToDbo(this InvoiceItem item)
    => new()
    {
        Uid = item.Uid,
        InvoiceUid = item.InvoiceUid,
        ItemQuantity = item.ItemQuantity,
        ItemUnitPrice = item.ItemUnitPrice,
        ProductUid = item.ProductUid,
    };

}
