/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public partial record InvoiceComposite
{
    public DiodeResult UpdateInvoice(DiodeMutationDelegate<InvoiceId, DmoInvoice> mutation, object? sender = null)
    {
        var result = _invoice.Update(mutation, sender);
        return result;
    }

    public bool DeleteInvoice()
    {
        _invoice.Delete(this);
        return true;
    }

    private void SetPersistedOnInvoice()
    {
            _invoice.Persisted(this);
    }

    private void OnInvoiceChanged(object? sender, DiodeEventArgs e)
    {
        if (!this.Equals(sender))
            this.UpdateInvoice(ApplyInvoiceRules, this);

        this.StateHasChanged?.Invoke(sender, EventArgs.Empty);
    }
}
