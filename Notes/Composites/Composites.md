# Composites

Composites are used to model complex entities.  They are often refered to as *Aggregates*.  I don't use that term because the *Composite* pattern implementation the same functionality a  little differently.

An invoice is the classic complex entity.  In this [simplistic] implementation it consists of an invoice:

```csharp
public record Invoice
{
    public InvoiceId InvoiceId { get; init; } = new(Guid.Empty);
    public CustomerId CustomerId { get; init; } = new(Guid.Empty);
    public decimal TotalAmount { get; init; }
    public DateOnly Date { get; init; }
}
```

And an invoice item:

```csharp
public record InvoiceItem
{
    public InvoiceItemId InvoiceItemId { get; init; } = new(Guid.Empty);
    public InvoiceId InvoiceId { get; init; } = new(Guid.Empty);
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
```


The two objects are tightly coupled.  An `InvoiceItem` only has meaning within the context of an `Invoice`.  Changing or adding an `InvoiceItem` affects the state of the parent `Invoice`: in our use case the `TotalAmount` needs to be recalculated.

The purpose of the composite is to provide a wrapper around the data that constitutes an invoice, control it's mutation and ensure the integrity of it's data.  If an `InvoiceItem` is added, deleted or changed, the `TotalAmount` of the `Invoice` needs to be recalculated.

The composite uses the *Blazr.FluxGate* library to provide a structured mutation process for the individual entities.

The primary objects are defined like this using a `FluxGateStore` to manage the `Invoice` instance and a `KeyedFluxGateStore` to manage the `invoiceItem` collection: 

```csharp
private FluxGateStore<DmoInvoice> _invoice;
private KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId> _invoiceItems;
```

The underlying Invoice and InvoiceItems are exposed as readonly objects:

```csharp
    public DmoInvoice Invoice => _invoice.Item;
    public IEnumerable<DmoInvoiceItem> AllInvoiceItems => _invoiceItems.Items;
    public IEnumerable<DmoInvoiceItem> InvoiceItems => _invoiceItems.Stores.Where(item => !item.State.IsDeleted).Select(item => item.Item).AsEnumerable();
```

Creating an `InvoiceComposite` requires the `Invoice` and `InvoiceItems` and access to the  `IServiceProvider`.  The FluxGateStores need to be created in the context of the Scoped Service provider.

The constructor uses the `ActivatorUtilities` utility class to construct instances of `FluxGateStore` and `KeyedFluxGateStore` within the context of the current DI `IServiceProvider`. 

```csharp
    public InvoiceComposite(IServiceProvider serviceProvider, DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, bool isNew = false)
    {
        _serviceProvider = serviceProvider;

        _invoice = (FluxGateStore<DmoInvoice>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<DmoInvoice>), new object[] { invoice, isNew });

        _invoice.StateChanged += this.OnInvoiceChanged;

        _invoiceItems = (KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId>));

        foreach (var item in invoiceItems)
        {
            var store = _invoiceItems.GetOrCreateStore(item.InvoiceItemId, item);
            store.StateChanged += this.OnInvoiceItemChanged;
        }
    }
```

The `Invoice` and its state are accessed via readonly properties:

```csharp
    public DmoInvoice Invoice => _invoice.Item;
    public FluxGateState State => _invoice.State;
```


Two methods provide query access to `Invoiceitems`:

```charp

    public DataResult<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId uid)
    {
        var item = _invoiceItems.Items.FirstOrDefault(item => item.InvoiceItemId == uid);
        return item is null ? DataResult<DmoInvoiceItem>.Failure("Item does not exist in store") : DataResult<DmoInvoiceItem>.Success(item);
    }

    public DataResult<FluxGateState> GetInvoiceItemState(InvoiceItemId uid)
    {
        var store = _invoiceItems.GetStore(uid);
        return store is null ? DataResult<FluxGateState>.Failure("Item does not exist in store") : DataResult<FluxGateState>.Success(store.State);
    }
```

A method to construct a new `InvoiceItem` within the `Invoice` context:
```csharp
    public DmoInvoiceItem GetNewInvoiceItem()
        => new() { InvoiceItemId = new(UUIDProvider.GetGuid()), InvoiceId = _invoice.Item.InvoiceId };
```

The dispatchers to mutate the `Invoice` and `InvoiceItems`.

```csharp
    public IDataResult DispatchInvoiceAction(IFluxGateAction action)
        => _invoice.Dispatch(action).ToDataResult();

    public IDataResult DispatchInvoiceItemAction(InvoiceItemId id, IFluxGateAction action)
    {
        if (action is AddInvoiceItemAction addAction)
            return this.AddInvoiceItem(addAction.Item);

        return _invoiceItems.Dispatch(id, action).ToDataResult();
    }
```

And a method to "Reset" the composite when it's persisted to the data store:

```csharp
    public void Persisted()
    {
        // persist the Invoice
        _invoice.Dispatch(new SetInvoiceAsPersistedAction(this));

        // get all the deleted items and remove them from the store
        foreach (var item in _invoiceItems.Items)
        {
            var store = _invoiceItems.GetStore(item.InvoiceItemId);
            if (store?.State.IsDeleted ?? false)
                _invoiceItems.RemoveStore(item.InvoiceItemId);
        }

        // Set the rest as persisted
        foreach (var item in _invoiceItems.Items)
            _invoiceItems.Dispatch(item.InvoiceItemId, new SetInvoiceItemAsPersistedAction(this));
    }
```

Internally the method to maintain composite consistency when something changes [in this case recalculate the total amount]:  

```csharp
    private void ApplyInvoiceRules()
    {
        decimal amount = 0;

        foreach (var item in this.InvoiceItems)
            amount = amount + item.Amount;

        _invoice.Dispatch(new UpdateInvoicePriceAction(this, amount));

        this.StateHasChanged?.Invoke(this, EventArgs.Empty);
    }
```

And two event handlers registered on the `FluxGateStore` objects to trigger the `ApplyInvoiceRules` method.

```csharp

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
```
