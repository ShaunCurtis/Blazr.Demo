# Commands

Generically we can define a command like this:

```csharp
CommandResult CommandAsync(Command command);
```

A generic command handler needs state tracking: we need the state of a record to know how to deal with it.

```csharp
public interface IStateEntity 
{ 
    public int StateCode { get; }
}
```

You can read more about State in the *Entity Objects* section.

The Command only needs to know three states:

1. Update - it will save a submitted record, replacing the existing data store record.  It doesn't care if it's mutated, that's the decision of the core domain code.
2. Delete - delete the record from the data store.
3. Add - Add the record to the data store.
 
Tracking state seems over-complicated for simple objects.  When I want to delete a weather forwcast, I just call delete on Entity Framework.

True, but complex objects require tracking.  If a user deletes an item from the shopping basket do you immediately delete the item from the database.  How does the shopping cart know it's been deleted: it needs to keep the total price updated.  What happens if the buyer changes his mind and wants to revert to his saved basket?

## The CommandEntity Interface

`ICommandEntity` is an empty interface.  It's purpose is to label those entities that allow independant Create/Update/Delete operations.  Some entities will be part of aggregates: they can only be Created/Updated/Deleted as part of the aggregate Create/Update/Delete operations. operations.   

## The Request

we can define a generic command request object:

```
public record struct CommandRequest<TRecord>(TRecord Item, CancellationToken Cancellation = new());
```

It's an immutable struct because it doesn't need to be anything more.  Almost all data pipelines are now async and implement cancellation, so our request defines a `CancellationToken` to allow cancellation of aborted commands.

## The Result

All commands only return status information.  We should never return a `null` without explaining why!

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
public sealed record CommandResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;

    private CommandResult() { }

    public static CommandResult Success(string? message = null)
        => new CommandResult { Successful = true, Message= message ?? string.Empty };

    public static CommandResult Failure(string message)
        => new CommandResult { Message = message};
}
```

There are two static constructors to control how a result is constructed: it either succeeded or failed.

## The Handler

The Core domain defines a *contract* interface that it uses to get items.  It doesn't care where they come from.  You may be implementing Blazor Server and calling directly into the database, or Blazor WASM and making API calls.

This is `ICommandHandler`.  There are two implementations: one for generic handlers and one for individual object based handlers.  They both define a single `ExecuteAsync` method.

```csharp
public interface ICommandHandler
{
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class, IStateEntity, new();
}

public interface ICommandHandler<TRecord>
        where TRecord : class, IStateEntity, new()
{
    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<TRecord> request);
}
```

### Server Handler

The handler basic structure looks like this.  `TDbContext` defines the `DbContext` to obtain through the DbContext Factory service.   

```csharp
public sealed class CommandServerHandler<TDbContext>
    : ICommandHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public CommandServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    { 
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class, IStateEntity, new()
    {
        //...
    }

    public async ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class, IStateEntity, new()
    {
        //...
    }
}
```

The default server method looks like this.  It gets a *unit of work* `DbContext` from the factory, calls the relevant Update/Add/Delete method on the context and returns a `CommandResult` based on the result.

```
    private async ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class, IStateEntity, new()
    {
        using var dbContext = _factory.CreateDbContext();

        var record = new TRecord();

        if ((record is not ICommandEntity))
            return CommandResult.Failure($"{record.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly.");

        var stateRecord = request.Item;

        if (StateCodes.IsUpdate(stateRecord.StateCode))
        {
            dbContext.Update<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Updated")
                : CommandResult.Failure("Error saving Record");
        }

        if (stateRecord.StateCode == StateCodes.New)
        {
            dbContext.Add<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Added")
                : CommandResult.Failure("Error adding Record");
        }

        if (stateRecord.StateCode == StateCodes.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Deleted")
                : CommandResult.Failure("Error deleting Record");
        }

        return CommandResult.Failure("Nothing executed.  Unrecognised StateCode.");
    }
```

The final method implements `ICommandHandler.ExecuteAsync`.  It checks to see if a specific `TRecord` implemented `IItemRequestHandler` is registered in the service container, and if so executes it instead of the default handler.

```csharp
    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class, IStateEntity, new()
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<ICommandHandler<TRecord>>();

        // If one exists execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If not run the base handler
        return await this.ExecuteCommandAsync<TRecord>(request);
    }
```


