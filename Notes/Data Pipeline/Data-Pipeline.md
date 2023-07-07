#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

The primary implementation is over Entity Framework, so we create a thin organisational broker layer. Entity Framework implements the basic repository and unit of work patterns for you, so don't wrap it in another fat layer. 

It's intention is to address persistence and retrieval in both a standards based generic context and complex aggregate contexts.  In summary, handle the 80% of cases that fit the standards approach, and provide the flexibility to handle the 20% custom cases.

## All data within the data pipeline is READONLY.

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
## All data within the data pipeline has Unique Identity.

`IGuidIdentity` is a simple guid interface.

```charp
public interface IGuidIdentity 
{ 
    public Guid Uid { get; }
}
```

## All data objects are state aware.

It's important in any application to understand and track the state of a record.  Not understsnding state is the cause of many coding problems.

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
