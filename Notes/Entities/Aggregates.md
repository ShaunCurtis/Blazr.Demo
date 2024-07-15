# Aggregate Objects

> This is dead content

Aggregates provide a structured framework for dealing with complex objects.  An invoice is a good example.

A invoice consists of an invoice object and a collection of invoice item objects.  The two are intimately linked: an invoice item does not exist outside the context of an invoice, an invoice is not an invoice without items to invoice.

In the database you may store these separately in tables, but in code they should be considered a single entity.

The single entity we use is an *AggregateRoot*.  Some treat the invoice as the aggregate, but I prefer to abstract further.

My Invoice aggregate class looks like this.  I'll look at the internal objects shortly.

```csharp
public sealed class InvoiceAggregate : IGuidIdentity, IStateEntity
{
    private readonly AggregateItemList<InvoiceItem> _invoiceItems = new(Enumerable.Empty<InvoiceItem>());
    private readonly AggregateItem<Invoice> _invoice; 

    public InvoiceAggregate() 
        => _invoice = AggregateItemFactory.AsNew(new Invoice());

    public InvoiceAggregate(Invoice invoice, IEnumerable<InvoiceItem>? items)
    {
        _invoice = AggregateItemFactory.AsExisting(invoice);
        _invoiceItems.Load(items);
    }

    ///...
}
```

As stated elsewhere, my data pipeline is:

 - readonly: all data objects are records.
 - state aware: all data objects have a `StateCode` property.

 These attributes soimplify implementing aggregate functionality.  We have simple processes to:

 - ensure they are immutable.
 - create copies.
 - compare objects.
 
 In the definition above the invoice and the invoice items are private.  No external classes have access to their functionality.  This highlights an important rule:

 > All functionality in the aggregate and it's data must be implemented through methods and properties in the top level aggregate root object.  Specifically, you should not be able to mutate sub objects directly through those objects.

Why?  Consider the Invoice total price.  If you could add/remove/change invoice items directly through the invoice item collection, how do you know when to recalculate the total price without creating a set of internal event and handler spaghetti?  A top down approach makes that simpler: call a top level `Calculate` method whenever you push a change down.

 Within our InvoiceAggregate object we expose:
 
  - the invoice through a property getter where the getter exposes a record i.e. an immutable data class.

 ```csharp
public Invoice Invoice => _invoice.Item;
```

 - the invoice items though a property getter which exposes an `IEnumerable` collection of records.  The items are immutable, and the collection can only be enumerated.  You can't add/delete/clear `SIEnumerable` objects.

 ```csharp
public IEnumerable<InvoiceItem> InvoiceItems
    => _invoiceItems.LiveInvoiceItems.AsEnumerable();
```

An example of a top level method is `RemoveInvoiceItem`.

Note that items are not deleted from collections: their `StateCode` is set to deleted.  Why?

 - We need to understand the state of the aggregate.  
 - We need to know how to persist the aggregate back to the data store.  Knowing which items to delete, add and update is important.
 - we need to be able to reset the aggregate or the items.

The method:
 - calls `SaveItem` on the invoice items collection.  
 - ensures the `StateCode` is set correctly.
 - calls `CalculateOnUpdate` on the aggregate to do any internal calculations or updates
 - returns a `CommandResult` which will contain an error message if the transaction failed.

```csharp
    public CommandResult RemoveInvoiceItem(InvoiceItem item)
    {
        var result = this._invoiceItems.SaveItem(item with { StateCode = StateCodes.Delete });

        if (result != null && result.Successful)
            this.CalculateOnUpdate();

        return result ?? CommandResult.Failure("No result returned.");
    }
```

On the `AggregateItemList` object `SaveItem` looks like this.  It:

 - gets the existing item from it's internal coillection is one exists.
 - calls `Update` on the AggregateItem if it exists
 - adds a new `AggregateItem` if it doesn't exist.

```csharp
public CommandResult SaveItem(TItem updateItem)
{
    var selectedItem = _items.FirstOrDefault(item => item.Uid == updateItem.Uid);
    if (selectedItem != null)
        selectedItem.Update(updateItem);

    else
        _items.Add(AggregateItemFactory.AsNew(updateItem));

    return CommandResult.Success();
}
```

In the `AggregateItem` object `Update` looks like this. It:

 - checks the Uid of the incoming item
 - replaces the `Item` record with the incoming record. 

```csharp
public CommandResult Update(TItem item)
{
    if (item.Uid != this.Item.Uid)
        return this.FailOnUidCheck;
    
    this.Item = item;
    return CommandResult.Success();
}
```

Let's look in a little more detail at the `AggregateItem` and `AggregateItemList`

### AggregateItem

The purpose of the `AggregateItem` is to track state.  It has no public constructors: you create instances through a factory class.

It has two internal properties:

```csharp
    public TItem? BaseItem { get; internal set; }
    public TItem Item { get; internal set; }
```

 - `BaseItem` is the copy of the data held in the data store.
 - `Item` is the current copy.

They are populated initially by the factory.

```csharp
public static class AggregateItemFactory
{
    public static AggregateItem<TItem> AsExisting<TItem>(TItem item)
        where TItem : class, IGuidIdentity, IStateEntity, new()
        => new AggregateItem<TItem>() { BaseItem = item, Item = item };

    public static AggregateItem<TItem> AsNew<TItem>(TItem item)
        where TItem : class, IGuidIdentity, IStateEntity, new()
        => new AggregateItem<TItem>() { BaseItem = null, Item = item };
}
```

 - `AsExisting` sets both `BaseItem` and `Item` to the incoming item.
 - `AsNew` sets just the `Item`.  There's no base item [which is why it's declared as nullable].

The other properties are:

 - `BaseStateCode` - which provides the StateCode for the base record
 - `Uid` - the record Guid
 - `IsDirty` - a simple record comparison to obtain the edit state

```csharp
    public int BaseStateCode => this.BaseItem?.StateCode ?? StateCodes.New; 
    public Guid Uid => this.Item.Uid;
    public bool IsDirty => this.Item != this.BaseItem;
```

The methods:

```csharp
    public CommandResult Update(TItem item);
    public CommandResult Update(TItem actualItem, TItem baseStateSetItem);
    public void Reset();
    public void SetAsSaved();
    public void SetAsNew(TItem newItem);
```

All are self explanatory except `Update(TItem actualItem, TItem baseStateSetItem)`.

It's purpose is to detect when the incoming record is equal to the base record except for the StatusCode.  This occurs when we change an edited item back to the original.  

We should do this internally, but we can't as we can't apply `record` specific functionality in generics.  Instead we move the `record` based copying to the parent aggregate object.  The caller can get the base `StateCode` from the `BaseStateCode` property and create a copy of the incoming item set to the `BaseStateCode`. This is the incoming `baseStateSetItem`.

If `BaseItem` and `baseStateSetItem` are the same, then the incoming record is clean and we can set `Item` to `baseStateSetItem`.  Otherwise we set it to the incoming `actualItem`. 

```csharp
public CommandResult Update(TItem actualItem, TItem baseStateSetItem)
{
    if (actualItem.Uid != this.Item.Uid)
        return this.FailOnUidCheck;

    var sameAsBase = BaseItem == baseStateSetItem;

    this.Item = sameAsBase
        ? baseStateSetItem
        : actualItem;

    return CommandResult.Success();
}
```

The full class skeleton:

```csharp
public class AggregateItem<TItem>
    where TItem : class, IGuidIdentity, IStateEntity, new()
{
    private CommandResult FailOnUidCheck = CommandResult.Failure("Can't update - the Uid of a submitted Item doesn't match the UI of the stored item.");

    public TItem? BaseItem { get; internal set; }
    public TItem Item { get; internal set; }
    public int BaseStateCode { get;} 
    public Guid Uid { get;}
    public bool IsDirty { get; }

    internal AggregateItem() { }

    public CommandResult Update(TItem item);
    public CommandResult Update(TItem actualItem, TItem baseStateSetItem);
    public void Reset();
    public void SetAsSaved();
    public void SetAsNew(TItem newItem);
}
```

## AggregateItemList

The purpose of `AggregateItemList` is to provide a list of `AggregateItem` and the methods to manage the list.

It maintains an internal `_items` list of `AggregateItem`, and exposes that list through two readonly `IEnumerable` properties:

 - `LiveInvoiceItems` contains all the unchanged, updated and new items.
 - `AllInvoiceItems` contains all the items including those marked for deletion.

 The public constructor requires an `IEnumerable` list to populate the internal list.

```csharp
public sealed class AggregateItemList<TItem>
    where TItem : class, IGuidIdentity, IStateEntity, new()
{
    private readonly List<AggregateItem<TItem>> _items;

    public IEnumerable<TItem> LiveInvoiceItems
        => _items
            .Where(item => item.Item.StateCode != StateCodes.Delete)
            .Select(item => item.Item)
            .AsEnumerable();

    public IEnumerable<TItem> AllInvoiceItems
        => _items
            .Select(item => item.Item)
            .AsEnumerable();

    public AggregateItemList(IEnumerable<TItem> items)
    {
        foreach (var item in items)
            _items.Add(AggregateItemFactory.AsExisting(item));
    }
}
```

The other property is `IsDirty` that checks for list to see if any items are dirty:

```cshaerp
    public bool IsDirty => _items.Any(item => item.IsDirty);
```

The methods are all fairly self evident.

```csharp
    public TItem? GetItem(Guid uid);
    public bool ItemExists(Guid uid);
    public CommandResult SaveItem(TItem updateItem);
    public CommandResult AddExistingItem(TItem newItem);
    public void Load(IEnumerable<TItem> items);
    public void ResetItem(TItem updateItem);
    public void ResetItems();
    public void SetAsSaved();
    public void Clear();
```

`SaveItem` updates existing items or adds an item if the incoming record does not exist.

```csharp
    public CommandResult SaveItem(TItem updateItem)
    {
        var selectedItem = _items.FirstOrDefault(item => item.Uid == updateItem.Uid);
        if (selectedItem != null)
            selectedItem.Update(updateItem);

        else
            _items.Add(AggregateItemFactory.AsNew(updateItem));

        return CommandResult.Success();
    }
```

The full class skeleton.

```csharp
public sealed class AggregateItemList<TItem>
    where TItem : class, IGuidIdentity, IStateEntity, new()
{
    private Guid Uid;
    private readonly List<AggregateItem<TItem>> _items;

    public bool IsDirty { get; }

    public IEnumerable<TItem> LiveInvoiceItems { get; }
    public IEnumerable<TItem> AllInvoiceItems { get; }

    public AggregateItemList(IEnumerable<TItem> items)
    {
        foreach (var item in items)
            _items.Add(AggregateItemFactory.AsExisting(item));
    }
    
    public TItem? GetItem(Guid uid);
    public bool ItemExists(Guid uid);

    public CommandResult SaveItem(TItem updateItem);
    public CommandResult AddExistingItem(TItem newItem);
    public void Load(IEnumerable<TItem> items);
    public void ResetItem(TItem updateItem);
    public void ResetItems();
    public void SetAsSaved();
    public void Clear();
}
```


### InvoiceAggregate

The `InvoiceAggregate` exposes the necessary functionality of the underlying objects.

Many of the methods and properties are simple pass through implementations.

For example `InvoiceItems`:

```csharp
public IEnumerable<InvoiceItem> InvoiceItems
    => _invoiceItems.LiveInvoiceItems.AsEnumerable();
```

And `DeleteInvoice`

```csharp
public CommandResult DeleteInvoice(Invoice invoice)
    => _invoice.Update(invoice with { StateCode = StateCodes.Delete });
```

Others such as `RemoveInvoiceItem` pass through and trigger internal processes.  In this case `CalculateOnUpdate` that recalculates the invoice price.

```csharp
public CommandResult RemoveInvoiceItem(InvoiceItem item)
{
    var result = this._invoiceItems.SaveItem(item with { StateCode = StateCodes.Delete });

    if (result != null && result.Successful)
        this.CalculateOnUpdate();

    return result ?? CommandResult.Failure("No result returned.");
}
```


The class skeleton.

```csharp
public sealed class InvoiceAggregate : IGuidIdentity, IStateEntity
{
    private readonly AggregateItemList<InvoiceItem> _invoiceItems;
    private readonly AggregateItem<Invoice> _invoice; 

    public Guid Uid { get; init; }
    public int StateCode { get; }
    public bool IsDirty { get; }

    public Invoice Invoice { get; }
    public IEnumerable<InvoiceItem> InvoiceItems { get; }
    public IEnumerable<InvoiceItem> AllInvoiceItems { get; }
    public InvoiceItem NewInvoiceItem { get; }

    public InvoiceAggregate();
    public InvoiceAggregate(Invoice invoice, IEnumerable<InvoiceItem>? items);

    public CommandResult UpdateInvoice(Invoice invoice);
    public CommandResult DeleteInvoice(Invoice invoice);

    public InvoiceItem GetInvoiceItem(ItemQueryRequest request);
    public CommandResult RemoveInvoiceItem(InvoiceItem item);
    public CommandResult SaveInvoiceItem(InvoiceItem item);
    public void ResetInvoiceItems();
    public void SetInvoiceAsSaved();
    public void ResetInvoice();
    public void SetToNew();
    private void CalculateOnUpdate();
    private bool InvoiceItemExists(Guid uid);
}
```

## The Data Pipeline

Our data pipeline has to deal differently with Aggregates.  Updated an invoice or invoice item should update the item in the aggregate, not update the item in the data store..

To accomplish this we need to implement custom Presenters and Handlers.  The data pipeline is designed to handle this. 

### The Presenters

At this point we have our data infrastructure in place, but we can't use the standard presenter for an invoice because we don't want to push the invoice into the data pipeline.  The presenter needs to point at the aggregate object.

This is relatively simple to implement.  We get the `InvoiceAggregateManager` through DI, and implement the calls into the `InvoiceAggregateManager` instance through `overridden` versions of `GetItemAsync` and `UpdateAsync`.

```csharp
public sealed class InvoiceEditPresenter : BlazrEditPresenter<Invoice, InvoiceEntityService, InvoiceEditContext>
{
    private InvoiceAggregateManager _aggregateManager;

    public InvoiceEditPresenter(IDataBroker dataBroker, INotificationService<InvoiceEntityService> notificationService, ILogger<InvoiceEditPresenter> logger, InvoiceAggregateManager aggregateManager)
        : base(dataBroker, notificationService, logger)
            => _aggregateManager = aggregateManager;

    protected override ValueTask GetItemAsync(ItemQueryRequest request)
    {
        LastResult = ItemQueryResult<Invoice>.Success(_aggregateManager.Record.Invoice);
        RecordContext.Load(_aggregateManager.Record.Invoice);
        this.EditContext = new EditContext(RecordContext);

        return ValueTask.CompletedTask;
    }

    protected override ValueTask UpdateAsync()
    {
        LastResult = CommandResult.Success();
        _aggregateManager.Record.UpdateInvoice(this.RecordContext.AsRecord);
        EditContext.SetEditStateAsSaved();
        this.LogResult();
        this.Notify();

        return ValueTask.CompletedTask;
    }
}
```

The `InvoiceItemEditPresenter` looks the same:

```csharp
public sealed class InvoiceItemEditPresenter : BlazrEditPresenter<InvoiceItem, InvoiceEntityService, InvoiceItemEditContext>
{
    private InvoiceAggregateManager _aggregateManager;

    public InvoiceItemEditPresenter(IDataBroker dataBroker, INotificationService<InvoiceEntityService> notificationService, ILogger<InvoiceItemEditPresenter> logger, InvoiceAggregateManager aggregateManager)
        : base(dataBroker, notificationService, logger)
            => _aggregateManager = aggregateManager;

    protected override ValueTask GetItemAsync(ItemQueryRequest request)
    {
        LastResult = ItemQueryResult<Invoice>.Success(_aggregateManager.Record.Invoice);
        var record = _aggregateManager.Record.GetInvoiceItem(request);
        RecordContext.Load(record);
        this.EditContext = new EditContext(RecordContext);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask UpdateAsync()
    {
        LastResult = CommandResult.Success();
        _aggregateManager.Record.SaveInvoiceItem(this.RecordContext.AsRecord);
        EditContext.SetEditStateAsSaved();
        this.LogResult();
        this.Notify();

        return ValueTask.CompletedTask;
    }
}
```

### Data Pipeline Handlers

The final bit of the jigsaw in the data pipeline are the handlers for the `InvoiceAggregate` data type.

The first is the `InvoiceCompositeItemRequestBaseServerHandler` item request handler.  This gets both the invoice and invoice items and returns a `ItemQueryResult<InvoiceAggregate>` object.

```csharp
public sealed class InvoiceCompositeItemRequestBaseServerHandler<TDbContext>
    : IItemRequestHandler<InvoiceAggregate>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public InvoiceCompositeItemRequestBaseServerHandler(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<ItemQueryResult<InvoiceAggregate>> ExecuteAsync(ItemQueryRequest request)
    {
        if (request == null)
            throw new DataPipelineException($"No ListQueryRequest defined in {GetType().FullName}");

        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        Invoice? invoice = new();

        var x = dbContext.Set<Invoice>();

        invoice = await dbContext.Set<Invoice>().SingleOrDefaultAsync(item => ((IGuidIdentity)item).Uid == request.Uid, request.Cancellation);

        if (invoice is null)
            return ItemQueryResult<InvoiceAggregate>.Failure("No record retrieved");

        List<InvoiceItem>? invoiceItems = await dbContext.Set<InvoiceItem>()
            .Where(item => item.InvoiceUid == invoice.Uid)
            .ToListAsync();

        InvoiceAggregate record = new(invoice, invoiceItems);

        return ItemQueryResult<InvoiceAggregate>.Success(record);
    }
}
```

The `InvoiceCompositeCommandHandler` deals with rthe rest of the CRUD operations.

It updates the Invoice if it's dirty and each item in the invoice items collection.  It deletes any items marked as deleted, updates and existing items that are dirty and adds any new items.

It applies all the transactions to a single DbContext save to ensure transactional integrity.

```csharp
public sealed class InvoiceCompositeCommandHandler<TDbContext> 
    : ICommandHandler<InvoiceAggregate>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;
    private ILogger<InvoiceCompositeCommandHandler<TDbContext>> _logger;

    public InvoiceCompositeCommandHandler(IDbContextFactory<TDbContext> factory, ILogger<InvoiceCompositeCommandHandler<TDbContext>> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<InvoiceAggregate> request)
    {
        if (request == null)
        {
            var message = $"No Save CommandRequest defined in {this.GetType().FullName}";
            _logger.LogError(message);
            throw new DataPipelineException(message);
        }

        using var dbContext = _factory.CreateDbContext();

        var invoiceData = request.Item;
        // Create the invoice if it's marked as new
        if (invoiceData.Invoice.StateCode == StateCodes.New)
            dbContext.Add<DboInvoice>(invoiceData.Invoice.ToDboInvoice());

        // Update the invoice if it is marked as modified)
        if (StateCodes.IsModified(invoiceData.Invoice.StateCode))
            dbContext.Update<DboInvoice>(invoiceData.Invoice.ToDboInvoice());

        // Delete the invoice if it is maarked as deleted)
        if (invoiceData.Invoice.StateCode == StateCodes.Delete)
            dbContext.Remove<DboInvoice>(invoiceData.Invoice.ToDboInvoice());

        // Update all the existing items
        foreach (var item in invoiceData.AllInvoiceItems)
        {
            if (item.StateCode == StateCodes.New)
            dbContext.Add<DboInvoiceItem>(item.ToDboInvoiceItem());

            if (StateCodes.IsModified(item.StateCode))
                dbContext.Update<DboInvoiceItem>(item.ToDboInvoiceItem());

            if (item.StateCode == StateCodes.Delete)
                dbContext.Remove<DboInvoiceItem>(item.ToDboInvoiceItem());
        }

        try
        {
            // Commit all changes as a singlw transaction
            var transactions = await dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateException)
        {
            var message = $"Failed to save the invoice {request.Item.Uid}.  Transaction aborted";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }
        catch (Exception e)
        {
            var message = $"An error occured trying to save invoice {request.Item.Uid}.  Detail: {e.Message}.";
            _logger.LogError(message);
            return CommandResult.Failure(message);
        }
    }
}
```










