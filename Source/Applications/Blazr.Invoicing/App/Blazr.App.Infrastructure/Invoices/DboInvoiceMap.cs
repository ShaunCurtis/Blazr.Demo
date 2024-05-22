/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class DboInvoiceMap : IDboEntityMap<DboInvoice, DmoInvoice>
{
    public DmoInvoice MapTo(DboInvoice item)
        => Map(item);

    public DboInvoice MapTo(DmoInvoice item)
        => Map(item);

    public static DmoInvoice Map(DboInvoice item)
        => new()
        {
            InvoiceId = new(item.InvoiceID),
            CustomerId = new(item.CustomerID),
            TotalAmount = item.TotalAmount,
            Date = DateOnly.FromDateTime(item.Date)
        };

    public static DboInvoice Map(DmoInvoice item)
        => new()
        {
            InvoiceID = item.InvoiceId.Value,
            CustomerID = item.CustomerId.Value,
            TotalAmount = item.TotalAmount,
            Date = item.Date.ToDateTime( TimeOnly.MinValue)
        };
}
