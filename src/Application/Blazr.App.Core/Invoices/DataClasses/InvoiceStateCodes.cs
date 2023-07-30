/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvoiceStateCodes : StateCodes
{
    public static StateCode Provisional = new(1, "Provisional");
    public static StateCode Submitted = new(21, "Submitted");
    public static StateCode Paid = new(1001, "Paid");

    public static List<StateCode> InvoiceStateCodeList = new()
    {
        Existing,
        New,
        Null,
        Provisional,
        Submitted,
        Paid
    };

    public static StateCode GetInvoiceStateCode(int code)
        => InvoiceStateCodeList.FirstOrDefault(item => item.Value == code);
}
