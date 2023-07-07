#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

The primary implementation is over Entity Framework, so we create a thin organisational broker layer. Entity Framework implements the basic repository and unit of work patterns for you, so don't wrap it in another fat layer. 

It's intention is to address persistence and retrieval in both a standards based generic context and complex aggregate contexts.  In summary, handle the 80% of cases that fit the standards approach, and provide the flexibility to handle the 20% custom cases.

## Data Objects

Before diving into the code, I need to explain some key data object concepts used in the code.

#### All data within the data pipeline is READONLY.

When you retrieve data from a data source it's a **copy** of the data within the data source.  It's not a pointer to the source data that you can mutate as you wish: it's read only.  To change the original, you pass a mutated copy of the original into the data store.

Implementing readonly objects is simple in C# 8+.  We have a value based object called a `record` and the `{ get; init; }` property definition.

We can declare our data object like this:

```
public sealed record Product : IGuidIdentity, IStateEntity
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;
    [NotMapped] public int StateCode { get; init; } = 1;
    public string ProductCode { get; init; } = "Not Set";
    public string ProductName { get; init; } = "Not Set";
    public decimal ProductUnitPrice { get; init; } = 0;
}
```

`IGuidIdentity` is a simple guid interface.

```charp
public interface IGuidIdentity 
{ 
    public Guid Uid { get; }
}
```

If you use the classic `int` or `long` Id fields in your database you can do:

```charp
public interface IIntIdentityRecord 
{ 
    public int Id { get; }
}

public interface ILongIdentityRecord 
{ 
    public long Id { get; }
}
```

If you use readonly objects, how do you edit them?  The answer is to create an edit context for the object, and derive a new record from the edit context and then submit the new record to the data store.  Editing is covered in the *EditContext* notes.

#### All data objects are state aware.

It's incredibly important in any application to understand and track the state of a record.  Not understsnding state is the cause of many coding problems.

State is managed by a `StateCode` property defined by the `IStateEntity` interface. 

```csharp
public interface IStateEntity 
{ 
    public int StateCode { get; }
}
```

In the product record above `StateCode` is not stored in the data store.

```csharp
    [NotMapped] public int StateCode { get; init; } = 1;
```

The state codes are defined in a static helper class as `consts`.

```csharp
public class StateCodes
{
    public const int Record = 1;
    public const int New = 0;
    public const int Delete = int.MinValue;

    public static bool IsModified(int value) => value < 0 && value is not int.MinValue;
    public static bool IsDirty(int value) => value < 0;
}
```

The `StateCode` provides process management.  The default is one state called Record.  We switch the state from positive to negative for clean/dirty status.

A record read from the database has a state of `1`.  When we create an edited copy of that object,we set the state to `-1`.  Any record with a state of `0` is a new record, and [as a convention] any record with the state set to the minimum value for an int is marked for deletion.

### Data Broker

We define the "contract" that the infrastructure domain code needs to honour as an `interface`.

```csharp
public interface IDataBroker
{
    public ValueTask<ListQueryResult<TRecord>> GetItemsAsync<TRecord>(ListQueryRequest request) where TRecord : class, new();
    public ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request) where TRecord : class, new();
    public ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request) where TRecord : class, new();
}
```

There are some key design points to take from this definition:

1. Generics are applied at the method level, not the class level.  There's one concrete implementation that implements the interface for all data classes.
2. Methods are passed a "Request" object and return a "Result" object.
3. Everything is Task based and returns a `ValueTask`.


The server implementation class looks like this.

For example the item request hander is defined as `IItemRequestHandler` and populated by DI in the constructor. `GetItemAsync` calls the `ExecuteAsync` method on the interface.  Each operation in the data broker has a corresponding handler defined by an interface and obtained through DI.


```csharp
public sealed class RepositoryDataBroker : IDataBroker
{
    private readonly IListRequestHandler _listRequestHandler;
    private readonly IItemRequestHandler _itemRequestHandler;
    private readonly ICommandHandler _commandHandler;

    public RepositoryDataBroker(IListRequestHandler listRequestHandler, IItemRequestHandler itemRequestHandler, CommandHandler commandHandler)
    {
        _listRequestHandler = listRequestHandler;
        _itemRequestHandler = itemRequestHandler;
        _commandHandler = commandHandler;
    }

    public ValueTask<ListQueryResult<TRecord>> GetItemsAsync<TRecord>(ListQueryRequest request) where TRecord : class, new()
        => _listRequestHandler.ExecuteAsync<TRecord>(request);

    public ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request) where TRecord : class, new()
        => _itemRequestHandler.ExecuteAsync<TRecord>(request);

    public ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request) where TRecord : class, new()
        => _commandHandler.ExecuteAsync<TRecord>(request);
}
```

### IItemRequestHandler

`IItemRequestHandler` handles the *R* of CRUD: read operations. `IItemRequestHandler` has both typed and non-typed definitions.

```csharp
public interface IItemRequestHandler
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, new();
}

public interface IItemRequestHandler<TRecord>
        where TRecord : class, new()
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync(ItemQueryRequest request);
}
```

### ItemRequestServerHandler

The server handler looks like this.  It injects the base Handler which we'll see shortly and the `IServiceProvider`.

The internal `_getItemAsync`:

1.  Gets a customer `IItemRequestHandler` registered for the specific record `TRecord` if one exists.
2.  If it finds one, it executes the custom handler and returns the result.
3.  If it doesn't, it executes the standard handler and returns the result.

The handler acts as a mediator and can be coded to do whatever actions you want on a record.  A custom handler can do validation checks through Fluent Validation, apply guards or object translations.

```csharp
public sealed class ItemRequestServerHandler<TDbContext>
    : IItemRequestHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ItemRequestDefaultServerHandler<TDbContext> _baseHandler;

    public ItemRequestServerHandler(IServiceProvider serviceProvider, ItemRequestDefaultServerHandler<TDbContext> serverHandler)
    {
        _serviceProvider = serviceProvider;
        _baseHandler = serverHandler;
    }

    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, new()
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<IItemRequestHandler<TRecord>>();

        // If we get one then one is registered in DI and we execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If there's no custom handler registered we run the base handler
        return await _baseHandler.ExecuteAsync<TRecord>(request);
    }
}
```
### ItemRequestDefaultServerHandler

The base generic handler that does the actual work.  It:
 
1. Gets a *unit of work* DbContext from the factory scoped to the method with `using`.
2. Turns off tracking: this is a query with no mutation.
3. Checks if `TRecord` implements `IGuidIdentity` i.e. has a `Guid` and it's Key/Identity field and if so constructs a query and gets the record.
4. If there no identity field it attempts to find the record through EF's `FindAsync` method.
5. Does various error checking during the process.
6. Returns a Success or Failure `ItemQueryResult` record containing the retrieved record.

```csharp
public sealed class ItemRequestDefaultServerHandler<TDbContext>
    : IItemRequestHandler
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public ItemRequestDefaultServerHandler(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, new()
    {
        if (request == null)
            throw new DataPipelineException($"No ListQueryRequest defined in {GetType().FullName}");

        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        TRecord? record = null;

        // first check if the record implements IGuidIdentity.  If so we can do a cast and then do the query via the Uid property directly 
        if (new TRecord() is IGuidIdentity)
            record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IGuidIdentity)item).Uid == request.Uid, request.Cancellation);

        // Try and use the EF FindAsync implementation
        else
            record = await dbContext.FindAsync<TRecord>(request.Uid);

        if (record is null)
            return ItemQueryResult<TRecord>.Failure("No record retrieved");

        return ItemQueryResult<TRecord>.Success(record);
    }
}
```

### Custom Handler

Custom handlers are for the 20%.  The handler below is for the `InvoiceAggregate`.  It populates the aggregate by querying the data store for both the invoice and it's associated invoice items.

```csharp
public sealed class InvoiceAggregateItemRequestHandler<TDbContext>
    : IItemRequestHandler<InvoiceAggregate>
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public InvoiceAggregateItemRequestHandler(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<ItemQueryResult<InvoiceAggregate>> ExecuteAsync(ItemQueryRequest request)
    {
        if (request == null)
            throw new DataPipelineException($"No ListQueryRequest defined in {GetType().FullName}");

        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        Invoice? invoice = new();

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

## The Command Handler

The command handler handles the **C**, **U** and **D** of **CRUD**.

The key to the simplicity is state.

1. It gets a *unit of work* DbContext from the factory.
2. It uses the `StateCode` to define which EF action to perform with the object.
3. It calls `SaveChangesAsync` against the context and expects and answer of `1`: one transaction change.
4. It returns the appropriate `CommandResult`.

```csharp
public sealed class CommandDefaultServerHandler<TDbContext>
    : ICommandHandler
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public CommandDefaultServerHandler(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class, new()
    {
        if (request is null)
            throw new DataPipelineException($"No CommandRequest defined in {this.GetType().FullName}");

        using var dbContext = _factory.CreateDbContext();

        var stateRecord = request.Item as IStateEntity;

        if (stateRecord is null)
            throw new DataPipelineException($"Record provided to {this.GetType().FullName} does not implement IStateEntity");

        var successMessage = "Record Updated";
        var failureMessage = "Error saving Record";

        if (StateCodes.IsModified(stateRecord.StateCode))
            dbContext.Update<TRecord>(request.Item);

        if (stateRecord.StateCode == StateCodes.New)
        {
            dbContext.Add<TRecord>(request.Item);
            successMessage = "Record Added";
            failureMessage = "Error adding Record";
        }

        if (stateRecord.StateCode == StateCodes.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            successMessage = "Record Deleted";
            failureMessage = "Error deleting Record";
        }

        return await dbContext.SaveChangesAsync(request.Cancellation) == 1
            ? CommandResult.Success(successMessage)
            : CommandResult.Failure(failureMessage);
    }
}
```

## The List Handler



