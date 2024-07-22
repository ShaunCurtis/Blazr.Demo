/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class NewInvoiceProvider : INewRecordProvider<DmoInvoice>
{
    public DmoInvoice NewRecord()
    {
        return new DmoInvoice() { InvoiceId = new(Guid.NewGuid()), Date = DateOnly.FromDateTime(DateTime.Now) };
    }
}