/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class DvoInvoiceMap : IDboEntityMap<DvoInvoice, DmoInvoice>
{
    public DmoInvoice MapTo(DvoInvoice item)
        => Map(item);

    public DvoInvoice MapTo(DmoInvoice item)
        => Map(item);

    public static DmoInvoice Map(DvoInvoice item)
        => new()
        {
            InvoiceId = new(item.InvoiceID),
            CustomerId = new(item.CustomerID),
            CustomerName = item.CustomerName,
            TotalAmount = item.TotalAmount,
            Date = DateOnly.FromDateTime(item.Date)
        };

    public static DvoInvoice Map(DmoInvoice item)
        => throw new DvoMappingException();
}
