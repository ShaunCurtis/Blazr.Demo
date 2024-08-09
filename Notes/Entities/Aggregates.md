# Aggregate Objects

> This is being updated

Aggregates provide a structured framework for dealing with complex objects.  An invoice is a good example.

An invoice consists of an invoice object and a collection of invoice item objects.  The two are intimately linked: an invoice item does not exist outside the context of an invoice, an invoice is not an invoice without items to invoice.

In the database you may store these in separate tables, but in your application design they should be considered a single entity.

The standard way of dealing with this is to make the invoice the *AggregateRoot* and build an aggregate object around the invoice.

I prefer to abstract further: I've called my objects *Composites* rather than *Aggregates* to avoid confusion.

The key atributes of a composite are:
1. It exposes readonly objects that represent the composite data.
1. It exposes Mwthods to mutate the state of the objects.
1. It tracks the change state of the data objects.

Some implemented technologies:

1. Records.  Everything is a `record` unless it specifically needs to mutate.  All data objects are defined as records.

1. The data pipeline is implemented in the `Blazr.OneWayStreet` library.  This includes a *Flux* based mutation implemenntation for objects and lists of objects.  See the *OneWayStreet Flux* note for more details.

If you defined an interface for the Invoice composite, it would look like this:


```csharp
public interface IInvoiceComposite
{
    DmoInvoice Invoice { get; }
    IEnumerable<DmoInvoiceItem> InvoiceItems { get; }

    bool IsNew { get; }
    FluxState State { get; }

    event EventHandler? StateHasChanged;

    bool DeleteInvoice();
    IDataResult UpdateInvoice(FluxMutationDelegate<InvoiceId, DmoInvoice> mutation, object? sender = null);

    DmoInvoiceItem? GetInvoiceItem(InvoiceItemId uid);
    FluxState GetInvoiceItemState(InvoiceItemId uid);

    bool AddInvoiceItem(DmoInvoiceItem invoiceItem);
    bool DeleteinvoiceItem(InvoiceItemId uid);
    IDataResult UpdateInvoiceItem(InvoiceItemId id, FluxMutationDelegate<InvoiceItemId, DmoInvoiceItem> mutation, object? sender = null);

    DmoInvoiceItem GetNewInvoiceItem();

    void Persisted();
}
```

With the class the Invoice and items are defined as:

```csharp
    private FluxContext<InvoiceId, DmoInvoice> _invoice;
    private List<FluxContext<InvoiceItemId, DmoInvoiceItem>> _invoiceItems = new();
```

And the external readonly properties:

```csharp
    public DmoInvoice Invoice => _invoice.Item;
    public IEnumerable<DmoInvoiceItem> InvoiceItems => _invoiceItems.Select(item => item.Item).AsEnumerable();
```

Flux state.  `IsNew` is a shortcut property used in Edit forms in the UI.

```csharp
    public FluxState State => _invoice.State;
    public bool IsNew => _invoice.State == FluxState.New;
```

External notification Event:

```csharp
    public event EventHandler? StateHasChanged;
```

The constructor populates the values, sets the ijtitla state and wired the individual Flux contexts to the `InvoiceItemChanged` handler.

```csharp
    public InvoiceComposite(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems, bool isNew = false)
    {
        var state = isNew ? FluxState.New : FluxState.Clean;

        _invoice = isNew
            ? FluxContext<InvoiceId, DmoInvoice>.CreateNew(invoice)
            : FluxContext<InvoiceId, DmoInvoice>.CreateClean(invoice);

        _invoice.StateHasChanged += this.OnInvoiceChanged;

        foreach (var item in invoiceItems)
        {
            var context = FluxContext<InvoiceItemId, DmoInvoiceItem>.CreateClean(item);
            _invoiceItems.Add(context);
            context.StateHasChanged += OnInvoiceItemChanged;
        }
    }
```

The next methods deal with updating and deleting the invoice through the Flux Context.

```csharp
    public IDataResult UpdateInvoice(FluxMutationDelegate<InvoiceId, DmoInvoice> mutation, object? sender = null);

    public bool DeleteInvoice();
```

And Getting, Adding and Deleting InvoiceItems through the Flux Contexts

```csharp
    public DmoInvoiceItem? GetInvoiceItem(InvoiceItemId uid);

    public DmoInvoiceItem GetNewInvoiceItem();

    public FluxState GetInvoiceItemState(InvoiceItemId uid);

    public bool AddInvoiceItem(DmoInvoiceItem invoiceItem);

    public IDataResult UpdateInvoiceItem(InvoiceItemId id, FluxMutationDelegate<InvoiceItemId, DmoInvoiceItem> mutation, object? sender = null);
    
    public bool DeleteinvoiceItem(InvoiceItemId uid);
```

Finally `Persisted` updates the Flux Context in the Invoice, removes any items marked as deleted and seta the Flux state on all the remaining items to `FluxState.New`.

```csharp
    public void Persisted()
    {
        _invoice.Persisted(this);

        var deletes = new List<FluxContext<InvoiceItemId, DmoInvoiceItem>>();

        foreach (var item in _invoiceItems)
        {
            if (item.State == FluxState.Deleted)
            {
                deletes.Add(item);
                continue;
            }

            item.Persisted(this);
        }

        foreach (var item in deletes)
            _invoiceItems.Remove(item);
    }
```




