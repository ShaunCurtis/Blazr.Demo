/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public partial record InvoiceComposite
{
    public DmoInvoiceItem? GetInvoiceItem(InvoiceItemId uid)
    {
        var context = _invoiceItems.FirstOrDefault(item => item.Id == uid);
        if (context is null)
            return null;

        return context.Item;
    }

    public DmoInvoiceItem GetNewInvoiceItem()
        => new() { InvoiceItemId = new(Guid.NewGuid()), InvoiceId = _invoice.Id };

    public DiodeState GetInvoiceItemState(InvoiceItemId uid)
    {
        var context = _invoiceItems.FirstOrDefault(item => item.Id == uid);
        if (context is null)
            return DiodeState.Clean;

        return context.State;
    }

    public bool AddInvoiceItem(DmoInvoiceItem invoiceItem)
    {
        if (this.GetInvoiceItem(invoiceItem.InvoiceItemId) is not null)
            return false;

        if (invoiceItem.InvoiceId != _invoice.Id)
            invoiceItem = invoiceItem with { InvoiceId = _invoice.Id };

        var context = DiodeContext<InvoiceItemId, DmoInvoiceItem>.CreateNew(invoiceItem);
        _invoiceItems.Add(context);
        context.StateHasChanged += OnInvoiceItemChanged;

        this.UpdateInvoice(ApplyInvoiceRules, this);
        this.StateHasChanged?.Invoke(this, EventArgs.Empty);

        return true;
    }

    public DiodeResult UpdateInvoiceItem(InvoiceItemId id, DiodeMutationDelegate<InvoiceItemId, DmoInvoiceItem> mutation, object? sender = null)
    {
        var context = _invoiceItems.FirstOrDefault(item => item.Id == id);
        if (context is null)
            return DiodeResult.Failure($"No Section found with ID: {id.Value} to apply mutation to.");

        var result = context.Update(mutation, sender);
        return result;
    }

    public bool DeleteinvoiceItem(InvoiceItemId uid)
    {
        var context = _invoiceItems.FirstOrDefault(item => item.Id == uid);

        if (context is null)
            return false;

        context.Delete();
        return true;
    }

    private void SetPersistedOnInvoiceItems()
    {
        var deletes = new List<DiodeContext<InvoiceItemId, DmoInvoiceItem>>();

        foreach (var item in _invoiceItems)
        {
            if (item.State == DiodeState.Deleted)
            {
                deletes.Add(item);
                continue;
            }

            item.Persisted(this);
        }

        foreach (var item in deletes)
            _invoiceItems.Remove(item);
    }

    private void OnInvoiceItemChanged(object? sender, DiodeEventArgs e)
    {
        if (!this.Equals(sender))
            this.UpdateInvoice(ApplyInvoiceRules, this);
    }
}
