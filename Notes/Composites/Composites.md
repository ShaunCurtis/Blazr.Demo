# Composites

Composites are used to represent complex objects.  In many discussions they will be refered to as *Aggregates*.

> I don't use the term *Aggregate*, because while the *Composite* pattern has the same purose, the impkementation is a little different.

An invoice is the classic complex object we'll use in this article.  It consists of a invoice.  A typical implement looks like this:

An Invoice:

```csharp
public record Invoice
{
    public InvoiceId InvoiceId { get; init; } = new(Guid.Empty);
    public CustomerId CustomerId { get; init; } = new(Guid.Empty);
    public decimal TotalAmount { get; init; }
    public DateOnly Date { get; init; }
}
```

And an Invoice Item

```csharp
public record InvoiceItem
{
    public InvoiceItemId InvoiceItemId { get; init; } = new(Guid.Empty);
    public InvoiceId InvoiceId { get; init; } = new(Guid.Empty);
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
```

The two objects are tightly bound.  An `InvoiceItem` only has meaning within the context of an `Invoice`.  Changing or adding an `InvoiceItem` affects the state of the parent `Invoice`: the `TotalAmount` needs to be recalculated.

The purpose of the composite is to provide a wrapper around the context objects, control mutation and ensure the integrity of the data within the composite.  If the quantity of an `InvoiceItem` changes, the `TotalAmount` of the `Invoice` is recalculated.

The composite uses *Blazr.FluxGate* to control and track mutstion od individual objects.

We can define our primary objects like this using a `FluxGateStore` to manage the `Invoice` instance and a `KeyedFluxGateStore` to manage the `invoiceItem` collection: 

```csharp
private FluxGateStore<DmoInvoice> _invoice;
private KeyedFluxGateStore<DmoInvoiceItem, InvoiceItemId> _invoiceItems;
```

The invoice and items are exposed as readonly objects:

```csharp
public DmoInvoice Invoice => _invoice.Item;
public IEnumerable<DmoInvoiceItem> InvoiceItems => _invoiceItems.Items;
```

