# Commands

Generically we can define a command like this:

```csharp
CommandResult CommandAsync(Command command);
```

To build a generic command handler we need to implement state tracking into our data pipeline.  We need to know the state of a record.  There are four base states: 

1. An existing record
2. An exiting mutated record i.e. it has the same identity as the record in the record store, but some or all of the data has changed.
3. It's a deleted record i.e. it still exists in the data store, but is marked in-memory for deletion.
4. It's a new record that currently only exists in-memory and has not yet bwen saved to the data store.
 
The need to track state seems over-complicated for simple objects.  When I want to delete a weathwr forwcast, I just call delete on Entity Framework.  True, but more complex objects require tracking.  When the user deletes an item from the shopping basket do you immediately delete the item from the database.  How does the shopping cart know it's been deleted: it needs to keep the total price updated.  What happens if the buyer changes his mind and wants to add it back in?

## The State Interface

The `IStateEntity` is defined with an `int` to track the state.

```
public interface IStateEntity 
{ 
    public int StateCode { get; }
}
```

And a `StateCodes` class defining some constants for the base codes.

```
public class StateCodes
{
    public const int Record = 1;
    public const int New = 0;
    public const int Delete = int.MinValue;
    public static bool IsModified(int value) => value < 0 && value is not int.MinValue;
    public static bool IsDirty(int value) => value <= 0;
}
```

Any state above zero is an existing record.  If that record is edited, it's state is the negative equivalent.  The base excisitng record state is `1` and if the object is mutated with respect to the stored data, it's `-1`.  When you actually presist it to the data store you presist the absolute value.  Why not juat have an integer to represent dirty?  I use `StateCode` in a broader context in process tracking, so for instance a ToDo item has these states:  

```csharp
public class ToDoStateCodes : StateCodes
{
    public const int Unassigned = 1;
    public const int Assigned = 2;
    public const int Closed = 3;
    public const int Completed = 4;
}
```

## The CommandEntity Interface

`ICommandEntity` is an empty interface.  It's purpose is to label those entities that allow independant Create/Update/Delete operations.  Some entities will be part of aggregates, and will only allow Create/Update/Delete as part of the aggregate Create/Update/Delete operations. operations.   

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


