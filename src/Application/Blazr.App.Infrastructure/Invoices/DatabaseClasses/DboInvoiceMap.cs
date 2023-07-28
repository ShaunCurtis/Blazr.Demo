/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class DboInvoiceMap : IDboEntityMap<DboInvoice, Invoice>
{
    public Invoice Map(DboInvoice item)
        => new()
        {
            InvoiceUid = new(item.Uid),
            EntityState = new(StateCodes.GetStateCode(item.StateCode)),
            InvoiceDate = item.InvoiceDate,
            InvoiceNumber = item.InvoiceNumber,
            InvoicePrice = item.InvoicePrice,
            CustomerUid = new(item.CustomerUid),
        };

    public DboInvoice Map(Invoice item)
    {
        var stateCode = item.EntityState.StateCode == InvoiceStateCodes.New
            ? InvoiceStateCodes.Provisional
            : item.EntityState.StateCode;

        return new()
        {
            Uid = item.Uid.Value,
            EntityState = item.EntityState,
            StateCode = stateCode.Value,
            InvoiceDate = item.InvoiceDate,
            InvoiceNumber = item.InvoiceNumber,
            InvoicePrice = item.InvoicePrice,
            CustomerUid = item.CustomerUid.Value,
        };
    }
}
