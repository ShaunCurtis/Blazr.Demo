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
            InvoiceUid = new(Guid.NewGuid()),
            CustomerUid = customer.CustomerUid,
            CustomerName = customer.CustomerName,
            InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
            InvoiceNumber = "--NEW--",
            EntityState = new(StateCodes.New),
        };

    public static InvoiceItem New(Invoice invoice)
        => new()
        {
            InvoiceItemUid = new(Guid.NewGuid()),
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceUid = invoice.InvoiceUid,
            EntityState = new(StateCodes.New),
        };

    public static InvoiceItem New(Invoice invoice, Product product, int quantity)
        => new()
        {
            InvoiceItemUid = new(Guid.NewGuid()),
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceUid = invoice.InvoiceUid,
            ProductUid = product.ProductUid,
            ProductName = product.ProductName,
            ProductCode = product.ProductCode,
            ItemUnitPrice = product.ProductUnitPrice,
            ItemQuantity = quantity,
            EntityState = new(StateCodes.New),
        };

    public static Invoice MutateState(Invoice item, StateCode state)
        => item with { EntityState = item.EntityState.Mutate(state) };

    public static InvoiceItem MutateState(InvoiceItem item, StateCode state)
        => item with { EntityState = item.EntityState.Mutate(state) };
}
