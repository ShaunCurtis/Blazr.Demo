# Commands

A command can be defined like this:

```csharp
CommandResult CommandAsync(CommandRequest command);
```

Commands have one of three actions: 

1. Update - overwrite the record in the data store.
2. Delete - delete the record from the data store.
3. Add - Add the record to the data store.
 
These are defined in a `CommandState` struct.  An enum is not used because the action state crosses domain boundaries and API interfaces.

```csharp
public readonly record struct CommandState
{
    public int Index { get; private init; } = 0;
    public string Value { get; private init; } = "None";

    public CommandState() { }

    public CommandState(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public static CommandState None = new CommandState(0, "None");
    public static CommandState Add = new CommandState(1, "Add");
    public static CommandState Update = new CommandState(2, "Update");
    public static CommandState Delete = new CommandState(-1, "Delete");

    public static CommandState GetState(int index)
        => (index) switch
        {
            1 => CommandState.Add,
            2 => CommandState.Update,
            -1 => CommandState.Delete,
            _ => CommandState.None,
        };
}
```

## The Request

we can define a generic command request object:

```
public readonly record struct CommandRequest<TRecord>(TRecord Item, CommandState State, CancellationToken Cancellation = new());
```

Most data pipelines are now async and implement cancellation, so our request object defines a `CancellationToken`.

However, this presents a problem for API requests, so we define an API version with mapping methods.

```csharp
public record struct CommandAPIRequest<TRecord>
{
    public TRecord? Item { get; init; }
    public int CommandIndex { get; init; }

    public CommandAPIRequest() { }

    public static CommandAPIRequest<TRecord> FromRequest(CommandRequest<TRecord> command)
        => new()
        {
            Item = command.Item,
            CommandIndex = command.State.Index
        };

    public CommandRequest<TRecord> ToRequest(CancellationToken? cancellation = null)
        => new()
        {
            Item = this.Item ?? default!,
            State = CommandState.GetState(this.CommandIndex),
            Cancellation = cancellation ?? CancellationToken.None
        };
}
```

## The Result

All commands only return status information.  It's bad practice to  return a `null` without explaining why!

First a very general interface to handle status information regardless of the type of data.

```csharp
public interface IDataResult
{
    public bool Successful { get; }
    public string Message { get; }
}
```

And two objects:

```csharp
public sealed record DataResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string? Message { get; init; }

    internal DataResult() { }

    public static DataResult Success(string? message = null)
        => new DataResult { Successful = true, Message= message };

    public static DataResult Failure(string message)
        => new DataResult { Message = message};

    public static DataResult Create(bool success, string? message = null)
        => new DataResult { Successful = success, Message = message };
}
```

```csharp
public sealed record DataResult<TData> : IDataResult
{
    public TData? Item { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    internal DataResult() { }

    public static DataResult<TData> Success(TData Item, string? message = null)
        => new DataResult<TData> { Successful = true, Item = Item, Message = message };

    public static DataResult<TData> Failure(string message)
        => new DataResult<TData> { Message = message };
}
```

We can now define `CommandResult`.

```csharp
public sealed record CommandResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string? Message { get; init; }
    public object? KeyValue { get; init; }

    public CommandResult() { }

    public static CommandResult Success(string? message = null)
        => new CommandResult { Successful = true, Message= message };

    public static CommandResult SuccessWithKey(object keyValue, string? message = null)
        => new CommandResult { Successful = true, KeyValue = keyValue, Message = message };

    public static CommandResult Failure(string message)
        => new CommandResult { Message = message};
}
```

Again we need an API version as we can't transmit generic objects correctly.

```csharp
public sealed record CommandAPIResult<TKey>
{
    public bool Successful { get; init; }
    public string? Message { get; init; }
    public TKey KeyValue { get; init; } = default!;

    public CommandAPIResult() { }

    public CommandResult ToCommandResult()
        => new()
        {
            Successful = Successful,
            Message = Message,
            KeyValue = KeyValue
        };
}
```

## The Handler

The Core domain defines a *contract* interface that it uses to get items.  It doesn't care where they come from.  You may be implementing Blazor Server and calling directly into the database, or Blazor WASM and making API calls.

This is `ICommandHandler`.  There are two implementations: a generic handler and a specific object based handler.  They both define a single `ExecuteAsync` method.

```csharp
public interface ICommandHandler
{
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class;
}

public interface ICommandHandler<TRecord>
        where TRecord : class
{
    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<TRecord> request);
}

public interface ICommandHandler<TRecord, TDbo>
        where TRecord : class
        where TDbo : class
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
        where TRecord : class
    {
        // Try and get a registered custom handler
        var _customHandler = _serviceProvider.GetService<ICommandHandler<TRecord>>();

        // If one exists execute it
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        // If not run the base handler
        return await this.ExecuteCommandAsync<TRecord>(request);
    }

    private async ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class
    {
        //...
    }
}
```

`ExecuteAsync` checks for a specific record registered handler.  If one exists it uses that, if not it calls the internal `ExecuteCommandAsync`.

The default server method looks like this.  It gets a *unit of work* `DbContext` from the factory, calls the relevant Update/Add/Delete method on the context and returns a `CommandResult` based on the result.

```
    private async ValueTask<CommandResult> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();

        if ((request.Item is not ICommandEntity))
            return CommandResult.Failure($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly.");

        var stateRecord = request.Item;

        // First check if it's new.
        if (request.State == CommandState.Add)
        {
            dbContext.Add<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Added")
                : CommandResult.Failure("Error adding Record");
        }

        // Check if we should delete it
        if (request.State == CommandState.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Deleted")
                : CommandResult.Failure("Error deleting Record");
        }

        // Finally it changed
        if (request.State == CommandState.Update)
        {
            dbContext.Update<TRecord>(request.Item);
            return await dbContext.SaveChangesAsync(request.Cancellation) == 1
                ? CommandResult.Success("Record Updated")
                : CommandResult.Failure("Error saving Record");
        }

        return CommandResult.Failure("Nothing executed.  Unrecognised State.");
    }
```
### API Handler

```csharp
public sealed class CommandAPIHandler
    : ICommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public CommandAPIHandler(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Uses a specific handler if one is configured in DI
    /// If not, uses a generic handler using the APIInfo attributes to configure the HttpClient request  
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        ICommandHandler<TRecord>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<ICommandHandler<TRecord>>();

        // Get the custom handler
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        return await CommandAsync<TRecord>(request);
    }


    public async ValueTask<CommandResult> CommandAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        var attribute = Attribute.GetCustomAttribute(typeof(TRecord), typeof(APIInfo));

        if (attribute is null || !(attribute is APIInfo apiInfo))
            throw new DataPipelineException($"No API attribute defined for Record {typeof(TRecord).Name}");

        using var http = _httpClientFactory.CreateClient(apiInfo.ClientName);

        var apiRequest = CommandAPIRequest<TRecord>.FromRequest(request);

        var httpResult = await http.PostAsJsonAsync<CommandAPIRequest<TRecord>>($"/API/{apiInfo.PathName}/Command", apiRequest, request.Cancellation);

        if (!httpResult.IsSuccessStatusCode)
            return CommandResult.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var commandAPIResult = await httpResult.Content.ReadFromJsonAsync<CommandAPIResult<Guid>>();

        CommandResult? commandResult = null;

        if (commandAPIResult is not null)
            commandResult = commandAPIResult.ToCommandResult();

        return commandResult ?? CommandResult.Failure($"No data was returned");
    }
}
```


