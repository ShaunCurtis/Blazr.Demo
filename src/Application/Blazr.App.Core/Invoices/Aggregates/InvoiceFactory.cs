/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class InvoiceFactory
{
    public static Invoice New(Customer customer)
        => new()
        {
            Uid = Guid.NewGuid(),
            CustomerName = customer.CustomerName,
            CustomerUid = customer.Uid,
            InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
            InvoiceNumber = "--NEW--",
            StateCode = 0
        };

    public static InvoiceItem New(Invoice invoice)
        => new()
        {
            Uid = Guid.NewGuid(),
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceUid = invoice.Uid,
            StateCode = 0
        };

    public static InvoiceItem New(Invoice invoice, Product product, int quantity)
        => new()
        {
            Uid = Guid.NewGuid(),
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceUid = invoice.Uid,
            ProductUid = product.Uid,
            ProductName = product.ProductName,
            ProductCode = product.ProductCode,
            ItemUnitPrice = product.ProductUnitPrice,
            ItemQuantity = quantity,
            StateCode = 0
        };

    public static Invoice MutateState(Invoice item, int state)
    => item with { StateCode = state };

    public static InvoiceItem MutateState(InvoiceItem item, int state)
    => item with { StateCode = state };
}
