# Item Requests

Generically we can define a request for a single item like this:

```csharp
ItemResult GetItemAsync(ItemRequest request);
```

To build a generic item request pipeline we need to define some structure to the data objects we retrieve.

## The Identity interface

An *Identity* is an object that can be identified by an unique identifier.  I now use `Guid` as my identity [Key] field for all my data classes [and database objects], so define a `IGuidIdentity` interface that all data objects implement.  If you still use `int` or other Id fields then define `IIntIdentity` or similar interfaces.

```
public interface IGuidIdentity 
{ 
    public Guid Uid { get; }
}
```

## The Request

With identity we can define a generic request object:

```
public readonly record struct ItemQueryRequest(Guid Uid, CancellationToken Cancellation = new ());
```

It's an immutable struct because it doesn't need to be anything more.  Almost all data pipelines are now async and implement cancellation, so our request defines a `CancellationToken` to allow cancellation of aborted queries.

## The Result

All requests return data and status information.  We should never return a `null` without explaining why!

We can define an interface so we handle the status information regardless of the type of data: we may want to display or log errors.

```csharp
public interface IDataResult
{
    public bool Successful { get; }
    public string Message { get; }
}
```

And then the result using generics.

```csharp
public sealed record ItemQueryResult<TRecord> : IDataResult
{
    public TRecord? Item { get; init;} 
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;

    private ItemQueryResult() { }

    public static ItemQueryResult<TRecord> Success(TRecord Item, string? message = null)
        => new ItemQueryResult<TRecord> { Successful=true, Item= Item, Message= message ?? string.Empty };

    public static ItemQueryResult<TRecord> Failure(string message)
        => new ItemQueryResult<TRecord> { Message = message};
}
```

There are two static constructors to control how a result is constructed: it either succeeded or failed.

## The Handler

The Core domain defines a *contract* interface that it uses to get items.  It doesn't care where they come from.  You may be implementing Blazor Server and calling directly into the database, or Blazor WASM and making API calls.

This is `IItemRequestHandler`.  There are two implementations: one for generic handlers and one for individual object based handlers.  They both define a single `ExecuteAsync` method.

```csharp
public interface IItemRequestHandler
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, IGuidIdentity, new();
}

public interface IItemRequestHandler<TRecord>
        where TRecord : class, IGuidIdentity, new()
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync(ItemQueryRequest request);
}
```

### Server Handler

The handler basic structure looks like this.  `TDbContext` defines the `DbContext` to obtain through the DbContext Factory service.   

```csharp
public sealed class ItemRequestServerHandler<TDbContext>
    : IItemRequestHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public ItemRequestServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, IGuidIdentity, new()
    {
        //...
    }

    private async ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request)
    where TRecord : class, IGuidIdentity, new()
    {
        //...
    }
}
```

The default server method looks like this.  It gets a *unit of work* `DbContext` from the factory, turns off tracking [this is only a query] anf gets the record through the `DbSet` in the `DbContext` using the provided `Uid`.  It returns an `ItemQueryResult` based on the result.

```
    private async ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest request)
    where TRecord : class, IGuidIdentity, new()
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => item.Uid == request.Uid, request.Cancellation);

        if (record is null)
            return ItemQueryResult<TRecord>.Failure($"No record retrieved with a Uid of {request.Uid}");

        return ItemQueryResult<TRecord>.Success(record);
    }
```

The final method implements `IItemRequestHandler.ExecuteAsync`.  It checks to see if a specific `TRecord` implemented `IItemRequestHandler` is registered in the service container, and if so executes it instead of the default handler.

```csharp
    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest request)
        where TRecord : class, IGuidIdentity, new()
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<IItemRequestHandler<TRecord>>();

        // If one is registered in DI execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If not run the base handler
        return await this.GetItemAsync<TRecord>(request);
    }
```



