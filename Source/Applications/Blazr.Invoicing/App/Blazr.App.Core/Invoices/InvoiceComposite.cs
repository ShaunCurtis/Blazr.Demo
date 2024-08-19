/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record InvoiceComposite
{
    private DiodeContext<InvoiceId, DmoInvoice> _invoice;
    private List<DiodeContext<InvoiceItemId, DmoInvoiceItem>> _invoiceItems = new();

    public DmoInvoice Invoice => _invoice.Item;
    public IEnumerable<DmoInvoiceItem> InvoiceItems => _invoiceItems.Select(item => item.Item).AsEnumerable();
    public DiodeState State => _invoice.State;
    public bool IsNew => _invoice.State == DiodeState.New;

    public event EventHandler? StateHasChanged;

    public InvoiceComposite(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, bool isNew = false)
    {
        var state = isNew ? DiodeState.New : DiodeState.Clean;

        _invoice = isNew
            ? DiodeContext<InvoiceId, DmoInvoice>.CreateNew(invoice)
            : DiodeContext<InvoiceId, DmoInvoice>.CreateClean(invoice);

        _invoice.StateHasChanged += this.OnInvoiceChanged;

        foreach (var item in invoiceItems)
        {
            var context = DiodeContext<InvoiceItemId, DmoInvoiceItem>.CreateClean(item);
            _invoiceItems.Add(context);
            context.StateHasChanged += OnInvoiceItemChanged;
        }
    }

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

    public void Persisted()
    {
        _invoice.Persisted(this);

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

    private void OnInvoiceChanged(object? sender, DiodeEventArgs e)
    {
        if (!this.Equals(sender))
            this.UpdateInvoice(ApplyInvoiceRules, this);

        this.StateHasChanged?.Invoke(sender, EventArgs.Empty);
    }

    private void OnInvoiceItemChanged(object? sender, DiodeEventArgs e)
    {
        if (!this.Equals(sender))
            this.UpdateInvoice(ApplyInvoiceRules, this);
    }

    private DiodeMutationResult<DmoInvoice> ApplyInvoiceRules(DiodeContext<InvoiceId, DmoInvoice> context)
    {
        decimal amount = 0;

        foreach (var item in this.InvoiceItems)
            amount = amount + item.Amount;

        var mutation = context.Item with { TotalAmount = amount };

        return DiodeMutationResult<DmoInvoice>.Success(mutation);
    }
}
