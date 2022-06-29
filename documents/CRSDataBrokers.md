# Building a Command/Query Data Pipeline for Blazor in DotNetCore

This article demonstrates how to build a DotNetCore data pipeline using a succinct CQS based data pipeline.

## The CQS Pattern

Many smaller projects avoid the CQS Data pipeline framework because it seen as complicated and required a large number of classes to implement.

Each data action is either a:

1. *Command* - an instruction to make a data change.
2. *Query* - an instruction to get some data.

Each action has a Command/Query class that defines the action and a Handler class to execute the action.

In essence:
- A *Request* object defines any information a *Handler* needs to execute the request and a *Result* (what it expects as a return value).

- A *Handler* object executes code to obtain the *Result* based on information in the *Request* object.   

In concept, it's very simple, and relatively easy to implement.  The problem is that most implementations are very verbose: lots of classes repeating the same old code.

This is an attempt to be succinct!

## Basic Interfaces and Classes

The methodology can be defined by two interfaces.

`ICQSRequest` defines any request:

1. It says the request produces an output defined as `TResult`.
2. It has a unique `TransactionId` to track the transaction (if required and implementated).

```csharp
public interface ICQSRequest<out TResult>
{
    public Guid TransactionId { get;}
}
```

`ICQSHandler` defines any handler that executes an `ICQSRequest` instance:

1. The handler gets a `TAction` which implements the `ICQSRequest` interface.
2. The handler outputs a `TResult` as defined in the `ICQSRequest` interface.
3. It has a single `ExecuteAsync` method that returns `TResult`.

```csharp
public interface ICQSHandler<in TAction, out TResult>
    where TAction : ICQSRequest<TResult>
{
    TResult ExecuteAsync();
}
```

## A Classic Implementation

Here's a classic implementatiion to add a `WeatherForecast` record:

`AddWeatherForecastCommand` is the request :

```csharp
public class AddWeatherForecastCommand
    : ICQSRequest<ValueTask<CommandResult>>
{
    public DboWeatherForecast Record { get; private set; } = default!;

    public AddWeatherForecastCommand(DboWeatherForecast record)
        => this.Record = record;
}
```

`AddWeatherForecastHandler` is the handler:

```csharp
public class AddWeatherForecastHandler
    : ICQSHandler<AddWeatherForecastCommand, ValueTask<CommandResult>>
{
    protected readonly IWeatherDbContext dbContext;
    protected readonly AddWeatherForecastCommand command;

    public AddWeatherForecastHandler(IWeatherDbContext dbContext, AddWeatherForecastCommand command)
    {
        this.command = command;
        this.dbContext = dbContext;
    }

    public async ValueTask<CommandResult> ExecuteAsync()
    {
        if (command.Record is not null)
            this.dbContext.DboWeatherForecast.Add(this.command.Record);

        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(Guid.Empty, true, "Record Saved")
            : new CommandResult(Guid.Empty, false, "Error saving Record");
    }
}
```

Relatively small classes, but think about implementing 3 sets (one each for add, update and delete) for each entity where we have *x* entities.  That's a lot of repetitive code.  The same principles apply to queries.  I see why people avoid using CQS!

## A Succinct Implementation

To build a more succinct implementation we need to accept:

 - The 80/20 rule.  Not every request can be fulfilled by our standardised implementation.  We need a custom route for these.
 - We need a "compliant" generics based ORM to interface with our data store.  The implementation uses *Entity Framework*. 
 - There's going to be some quite complicated generics implemented.

## Results

Before we dive into requests and handlers, we need to define a set of standard results they return: the `TResult` of the request.  Each is a `struct` containing status information and, if a request, the requested information.  They're self explanatory.
```csharp
public readonly struct ListProviderResult<TRecord>
{
    public IEnumerable<TRecord> Items { get; }
    public int TotalItemCount { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```
```csaharp
public readonly struct RecordProviderResult<TRecord>
{
    public TRecord? Record { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```
```csaharp
public readonly struct RecordCountProviderResult
{
    public int Count { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```
```csaharp
public readonly struct CommandResult
{
    public Guid NewId { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; }
    //....Constructors
}
```
```csaharp
public readonly struct FKListProviderResult
{
    public IEnumerable<IFkListItem> Items { get; }
    public bool Success { get; }
    public string? Message { get; }
    //....Constructors
}
```

## Base Classes

`TRecord` represents data classes retrieved from the data store using the ORM.

### Commands

All commands:

1. Take a record which we can define as `TRecord`.
2. Return an async `CommandResult`, so we can fix `TResult`.

First an interface that implements `ICQSRequest` and this functionality.

```csharp
public interface IRecordCommand<TRecord> 
    : ICQSRequest<ValueTask<CommandResult>>
{
    public TRecord Record { get;}
}
```

And an abstract implementation.

```csharp
public abstract class RecordCommandBase<TRecord>
     : IRecordCommand<TRecord>
{
    public Guid TransactionId { get; } = Guid.NewGuid(); 
    public TRecord Record { get; protected set; } = default!;

    public RecordCommandBase(TRecord record)
        => this.Record = record;
}
```

We can now define our Add/Delete/Update specific commands.

```csharp
public class AddRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    public AddRecordCommand(TRecord record) : base(record)
    {}
}
public class DeleteRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    public DeleteRecordCommand(TRecord record) : base(record)
    {}
}
public class UpdateRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    public UpdateRecordCommand(TRecord record) : base(record)
    {}
}
```

### The Handler

There's no benefits in creating interfaces or base classes so we implement Create/Update/Delete commands as three classes.  There are a lot of generics involved.  `TRecord` defines the record class and TDbContext the `DbContext` used in the DI `DbContextFactory`.

We don't need the know the actual `DbContext` class bwcuase we use the generic `Set<TRecord>` method in `DBContext` to find the `DbSet` instances of `TRecord` and the generic `Update<TRecord>`, `Add<TRecord>` and `Delete<TRecord>` methods with `SaveChangesAsync` for the commands. 

```csharp
public class AddRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<AddRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;
    protected readonly AddRecordCommand<TRecord> command;

    public AddRecordCommandHandler(IDbContextFactory<TDbContext> _factory, AddRecordCommand<TRecord> command)
    {
        this.command = command;
        this.factory = _factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync()
    {
        using var dbContext = factory.CreateDbContext();
        dbContext.Add<TRecord>(command.Record);
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(Guid.Empty, true, "Record Saved")
            : new CommandResult(Guid.Empty, false, "Error saving Record");
    }
}
``` 
```csharp
public class UpdateRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<UpdateRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;
    protected readonly UpdateRecordCommand<TRecord> command;

    public UpdateRecordCommandHandler(IDbContextFactory<TDbContext> _factory, UpdateRecordCommand<TRecord> command)
    {
        this.command = command;
        this.factory = _factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync()
    {
        using var dbContext = factory.CreateDbContext();
        dbContext.Update<TRecord>(command.Record);
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(Guid.Empty, true, "Record Saved")
            : new CommandResult(Guid.Empty, false, "Error saving Record");
    }
}
```
```csharp
public class DeleteRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<DeleteRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;
    protected readonly IRecordCommand<TRecord> command;

    public DeleteRecordCommandHandler(IDbContextFactory<TDbContext> _factory, IRecordCommand<TRecord> command)
    {
        this.command = command;
        this.factory = _factory;
    }

    public async ValueTask<CommandResult> ExecuteAsync()
    {
        using var dbContext = factory.CreateDbContext();
        dbContext.Remove<TRecord>(command.Record);
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(Guid.Empty, true, "Record Saved")
            : new CommandResult(Guid.Empty, false, "Error saving Record");
    }
}
```

## ICQSDataBroker/CQSDataBroker

We can now define a factory class to abstract the execution of *Requests* against their respective *Handlers*.

```csharp
public interface ICQSDataBroker
{
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new();
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new();
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new();
    //.... other ExecuteAsyncs
    public ValueTask<object> ExecuteAsync<TRecord>(object query);
}
```

```csharp
public class CQSDataBroker<TDbContext>
    :ICQSDataBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public CQSDataBroker(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new AddRecordCommandHandler<TRecord, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new UpdateRecordCommandHandler<TRecord, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new DeleteRecordCommandHandler<TRecord, TDbContext>(_factory, command);
        var result = await handler.ExecuteAsync();
        return result;
    }
    //.... other ExecuteAsyncs

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();
}
```

### Queries

Queries present a different challenge.

1. There are various types of `TResult`.
2. Specific *Where* and *OrderBy* list queries.

We define three Query requests:

### RecordQuery

This returns a `RecordProviderResult` containing a single record based on a provide Id.

```csharp
public record RecordQuery<TRecord>
    : ICQSRequest<ValueTask<RecordProviderResult<TRecord>>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    public readonly Guid? RecordId;

    public RecordQuery(Guid? recordId)
        => this.RecordId = recordId;
}
```

### RecordListQuery

This returns a `ListProviderResult` containing a *paged* `IEnumerable` of `TRecord`.

```csharp
public record RecordListQuery<TRecord>
    : ICQSRequest<ValueTask<ListProviderResult<TRecord>>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    public ListProviderRequest Request { get; protected set; }

    public RecordListQuery(ListProviderRequest request)
        => Request = request;
}
```

### FKListQuery

This returns a `FkListProviderResult` containing an `IEnumerable` of `IFkListItem`.  A `FkListItem` is a simple object containing a *Guid/Name* pair that can be used to populate foreign key *Select* controls in the UI.



```csharp
public record FKListQuery<TRecord>
    : ICQSRequest<ValueTask<FKListProviderResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
}
```

## Handlers

The coresponding handlers are:

### RecordQueryHandler

The key concepts to note her are:

1. The use of *unit of work* `DbContexts` from the `IDbContextFactory`.
2. `_dbContext.Set<TRecord>()` to get the `DbSet` for `TRecord`.
3. The use of reflection to get the `Key` property od the data class.

```csharp
public class RecordQueryHandler<TRecord, TDbContext>
    : ICQSHandler<RecordQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext

{
    private readonly RecordQuery<TRecord> _query;
    private IDbContextFactory<TDbContext> _factory;
    private bool _success = true;
    private string _message = string.Empty;

    public RecordQueryHandler(IDbContextFactory<TDbContext> factory, RecordQuery<TRecord> query)
    {
        _factory = factory;
        _query = query;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<RecordProviderResult<TRecord>> _executeAsync()
    {
        var _dbContext = _factory.CreateDbContext();
        TRecord? record = null;
        if (GetKeyProperty(out PropertyInfo? value) && value is not null)
        {
            record = await _dbContext.Set<TRecord>()
                .SingleOrDefaultAsync(item => GuidCompare(value.GetValue(item)));

            if (record is null)
            {
                _message = "No record retrieved";
                _success = false;
            }
        }
        return new RecordProviderResult<TRecord>(record, _success, _message);
    }

    private bool GuidCompare(object? value)
        => value is Guid && (Guid)value == _query.RecordId;

    private bool GetKeyProperty(out PropertyInfo? value)
    {
        var instance = new TRecord();
        value = instance.GetType()
            .GetProperties()
            .FirstOrDefault(prop => prop.GetCustomAttributes(false)
                .OfType<KeyAttribute>()
                .Any());
        if (value is null)
        {
            _message = "No Key attribute defined for the data set";
            _success = false;
        }

        return value is not null;
    }
}
```

### RecordListQueryHandler

The key concepts to note her are:

1. The use of *unit of work* `DbContexts` from the `IDbContextFactory`.
2. `_dbContext.Set<TRecord>()` to get the `DbSet` for `TRecord`.
3. The use of `IQuerable` to build queries before executing them.

```csharp
public class RecordListQueryHandler<TRecord, TDbContext>
    : ICQSHandler<RecordListQuery<TRecord>, ValueTask<ListProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;
    protected IDbContextFactory<TDbContext> factory;
    protected readonly RecordListQuery<TRecord> listQuery;

    public RecordListQueryHandler(IDbContextFactory<TDbContext> factory, RecordListQuery<TRecord> query)
    {
        this.factory = factory;
        this.listQuery = query;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<ListProviderResult<TRecord>> _executeAsync()
    {
        if (this.listQuery is null)
            return new ListProviderResult<TRecord>(new List<TRecord>(), 0, false, "No Query Defined");

        if (await this.GetItemsAsync())
            await this.GetCountAsync();

        return new ListProviderResult<TRecord>(this.items, this.count);
    }

    protected virtual async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        IQueryable<TRecord> dbSet = dbContext.Set<TRecord>();
        if (listQuery.Request.PageSize > 0)
            dbSet = dbSet
                .Skip(listQuery.Request.StartIndex)
                .Take(listQuery.Request.PageSize);

        this.items = await dbSet.ToListAsync();
        return true;
    }

    protected virtual async ValueTask<bool> GetCountAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        IQueryable<DvoWeatherForecast> dbSet = dbContext.Set<DvoWeatherForecast>();
        count = await dbSet.CountAsync();
        return true;
    }
}
```

### FKListQueryHandler

```csharp
public class FKListQueryHandler<TRecord, TDbContext>
    : ICQSHandler<FKListQuery<TRecord>, ValueTask<FKListProviderResult>>
        where TDbContext : DbContext
        where TRecord : class, IFkListItem, new()
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected IDbContextFactory<TDbContext> factory;
    protected readonly FKListQuery<TRecord> listQuery;

    public FKListQueryHandler(IDbContextFactory<TDbContext> factory, FKListQuery<TRecord> query)
    {
        this.factory = factory;
        this.listQuery = query;
    }

    public async ValueTask<FKListProviderResult> ExecuteAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        if (listQuery is null)
            return new FKListProviderResult(Enumerable.Empty<IFkListItem>(), false, "No Query defined");

        IEnumerable<TRecord> dbSet = await dbContext.Set<TRecord>().ToListAsync();
        return new FKListProviderResult(dbSet);
    }
}
```

## ICQSDataBroker/CQSDataBroker

We can now define a factory class to abstract the execution of *Requests* against their respective *Handlers*.

```csharp
public interface ICQSDataBroker
{
    public ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordListQuery<TRecord> query) where TRecord : class, new();
    public ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new();
    public ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new();
    //.... other ExecuteAsyncs
    public ValueTask<object> ExecuteAsync<TRecord>(object query);
}
```

```csharp
public class CQSDataBroker<TDbContext>
    :ICQSDataBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public CQSDataBroker(IDbContextFactory<TDbContext> factory)
        => _factory = factory;

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordListQuery<TRecord> query) where TRecord : class, new()
    {
        var handler = new RecordListQueryHandler<TRecord,TDbContext>(_factory, query);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new()
    {
        var handler = new RecordQueryHandler<TRecord, TDbContext>(_factory, query);
        var result = await handler.ExecuteAsync();
        return result;
    }

    public async ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new()
    {
        var handler = new FKListQueryHandler<TRecord, TDbContext>(_factory, query);
        var result = await handler.ExecuteAsync();
        return result;
    }
    //.... other ExecuteAsyncs

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();
}
```

## Testing

We can write a set of simple tests to test our pipeline.  These build out a DI service cointainer rather than use Mock. 

Note:

1. WeatherTestDataProvider is a singleton class that provides a set of Test Data
2. `GetServiceProvider` builds out a DI container with the necessary DI classes loaded.

```csharp
    private WeatherTestDataProvider _weatherTestData;

    public CQSBrokerTests()
    {
        _weatherTestData = WeatherTestDataProvider.Instance();
    }

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<InMemoryWeatherDbContext>(options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        var provider = services.BuildServiceProvider();

        var db = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>()!;
        _weatherTestData.LoadDbContext(db);

        return provider!;
    }
```

Here's the Update test method as an example.

```csharp
[Fact]
public async void TestUpdateCQSDataBroker()
{
    var provider = GetServiceProvider();
    var broker = provider.GetService<ICQSDataBroker>()!;

    var cancelToken = new CancellationToken();
    var listRequest = new ListProviderRequest(0, 1000, cancelToken);
    var query = new RecordListQuery<DvoWeatherForecast>(listRequest);

    var startRecords = await broker.ExecuteAsync(query);

    var editedRecord = _weatherTestData.GetRandomRecord()! with { Date = DateTime.Now.AddDays(10) };
    var editedDvoRecord = _weatherTestData.GetDvoWeatherForecast(editedRecord);
    var id = editedRecord.WeatherForecastId;

    var command = new UpdateRecordCommand<DboWeatherForecast>(editedRecord);
    var result = await broker.ExecuteAsync(command);

    var recordQuery = new RecordQuery<DvoWeatherForecast>(id);
    var updatedRecord = await broker.ExecuteAsync(recordQuery);

    Assert.True(result.Success);
    Assert.Equal(editedDvoRecord, updatedRecord.Record);
}
```
