/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public partial record InvoiceComposite
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

    public void Persisted()
    {
        this.SetPersistedOnInvoice();
        this.SetPersistedOnInvoiceItems();
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
