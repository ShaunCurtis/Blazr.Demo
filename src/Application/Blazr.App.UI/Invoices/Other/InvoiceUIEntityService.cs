/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
 
namespace Blazr.App.UI;

public sealed record InvoiceUIEntityService : IUIEntityService<InvoiceEntityService>
{
    public string SingleDisplayName { get; } = "Invoice";
    public string PluralDisplayName { get; } = "Invoices";
    public Type? EditForm { get; } = typeof(InvoiceEditForm);
    public Type? ViewForm { get; } = typeof(InvoiceViewForm);
    public string Url { get; } = "/invoice";
}
