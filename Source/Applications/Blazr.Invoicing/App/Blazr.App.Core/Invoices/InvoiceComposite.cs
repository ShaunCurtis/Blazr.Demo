/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.FluxGate;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Core;

public class InvoiceComposite
{
    private FluxGateStore<DmoInvoice> _invoice;
    private KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId> _invoiceItems;
    private readonly IServiceProvider _serviceProvider;
    private bool _isLoaded;
    public DmoInvoice Invoice => _invoice.Item;
    public IEnumerable<DmoInvoiceItem> InvoiceItems => _invoiceItems.Items;
    public FluxGateState State => _invoice.State;

    public event EventHandler? StateHasChanged;

    public InvoiceComposite(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _invoice = (FluxGateStore<DmoInvoice>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<DmoInvoice>));
        _invoiceItems = (KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId>));
    }

    public InvoiceComposite(IServiceProvider serviceProvider, DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, bool isNew = false)
    {
        _serviceProvider = serviceProvider;

        // both set in Load
        _invoice = default!;
        _invoiceItems = default!;

        this.Load(invoice, invoiceItems, isNew);
    }

    public IDataResult Load(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, bool isNew = false)
    {
        if (_isLoaded)
            return DataResult.Failure("The Invoice is already loaded");

        _invoice = (FluxGateStore<DmoInvoice>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<DmoInvoice>), new object[] { invoice, isNew });

        _invoice.StateChanged += this.OnInvoiceChanged;

        _invoiceItems = (KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId>));

        foreach (var item in invoiceItems)
        {
            var store = _invoiceItems.GetOrCreateStore(item.InvoiceItemId, item);
            store.StateChanged += this.OnInvoiceItemChanged;
        }

        _isLoaded = true;

        return DataResult.Success();
    }

    public void Persisted()
    {
        // persist the Invoice
        _invoice.Dispatch(new SetInvoiceAsPersistedAction(this));

        // get all the deleted items and remove them from the store
        foreach (var item in _invoiceItems.Items)
        {
            var store = _invoiceItems.GetStore(item.Id);
            if (store?.State.IsDeleted ?? false)
                _invoiceItems.RemoveStore(item.Id);
        }

        // Set the rest as persisted
        foreach (var item in _invoiceItems.Items)
            _invoiceItems.Dispatch(item.Id, new SetInvoiceItemAsPersistedAction(this));
    }

    public IDataResult DispatchInvoiceAction(IFluxGateAction action)
        => _invoice.Dispatch(action).ToDataResult();

    public IDataResult DispatchInvoiceItemAction(InvoiceItemId id, IFluxGateAction action)
    {
        if (action is AddInvoiceItemAction addAction)
            return this.AddInvoiceItem(addAction.Item);

        return _invoiceItems.Dispatch(id, action).ToDataResult();
    }

    public DmoInvoiceItem GetNewInvoiceItem()
        => new() { InvoiceItemId = new(Guid.NewGuid()), InvoiceId = _invoice.Item.Id };

    public DataResult<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId uid)
    {
        var item = _invoiceItems.Items.FirstOrDefault(item => item.Id == uid);
        return item is null ? DataResult<DmoInvoiceItem>.Failure("Item does not exist in store") : DataResult<DmoInvoiceItem>.Success(item);
    }

    public DataResult<FluxGateState> GetInvoiceItemState(InvoiceItemId uid)
    {
        var store = _invoiceItems.GetStore(uid);

        return store is null ? DataResult<FluxGateState>.Failure("Item does not exist in store") : DataResult<FluxGateState>.Success(store.State);
    }

    private IDataResult AddInvoiceItem(DmoInvoiceItem item)
    {
        if (_invoiceItems.GetStore(item.Id) is not null)
            return DataResult.Failure($"An item already exists with Id: {item.Id}.");

        var store = _invoiceItems.GetOrCreateStore(item.InvoiceItemId, item, true);
        store.StateChanged += this.OnInvoiceItemChanged;
        this.OnInvoiceChanged(null, new FluxGateEventArgs());
        return DataResult.Success();
    }

    private void OnInvoiceChanged(object? sender, FluxGateEventArgs e)
    {
        if (!this.Equals(sender))
            this.ApplyInvoiceRules();
    }

    private void OnInvoiceItemChanged(object? sender, FluxGateEventArgs e)
    {
        if (!this.Equals(sender))
            this.ApplyInvoiceRules();
    }

    private void ApplyInvoiceRules()
    {
        decimal amount = 0;

        foreach (var item in this.InvoiceItems)
            amount = amount + item.Amount;

        _invoice.Dispatch(new UpdateInvoicePriceAction(this, amount));

        this.StateHasChanged?.Invoke(this, EventArgs.Empty);
    }
}
