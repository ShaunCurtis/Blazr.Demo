> Old Version
> 
# Building a Command/Query Data Pipeline for Blazor in DotNetCore

The first time I came across CQS - the answer to my problems with the Repository pattern.  My excitment quickly disipated when I discovered how many classes I needed to create to implement it.  My generic repository framework, even with it's design problems, trumped it hands down.

I recently had cause to revisit CQS on an application re-write and decided to work on creating a more succinct version.  This article is about what I managed to achieve.

CQS - not to be confused with CQRS - is fundimentally a programming style.  Every action is either:

1. A *Command* - a request to make a data change.
2. A *Query* - a request to get some data.

A *Command* returns either status information or nothing.  CQS defines it **NEVER** return a data set.

A *Query* returns a data set.  CQS defines it **NEVER** makes makes changes to the state of the data.  Ther are no **NO SIDE EFFECTS** to the owning object.

Good practice is to apply theese rules to all object methods.  They make code more readable to the third person.

## Code Repository

> Not to be confused with the Repository pattern


You can find the code used in this article in my consolidated **Blazr.Demo** repository here: 

 - [Blazr.Demo Repository](https://github.com/ShaunCurtis/Blazr.Demo)

Note that this repo also contains a generic *Repository* framework data pipeline and several Blazor SPA implementations demonstrating various Blazor concepts and practices.

> Note: The solution has `Nullable` enabled, which explains some of the coding practices.

## The CQS Pattern

Smaller projects tend to avoid the CQS Data pipeline framework: it's seen as complicated because the demo implementations contain a lot of classes.

Each action has a *Command/Query* class that defines the action and a *Handler* class to execute the defined action.  Normally there's a one-to-one relationship:  a unique handler for every request.

In essence:

- A *Request* object defines the information a *Handler* needs to execute the request and what it expects in return - the *Result* .

- A *Handler* object executes the necessary code and returns the defined *Result* using data provided by the *Request*.  

Conceptually it's very simple, and relatively easy to implement.  The problem is most implementations are very verbose: lots of classes repeating the same old code.  Here's an example:

![Verbose CQS](./images/verbose-cqs.png)

*Sincere apologies if you recognise this as your code!  I searched Google for an example and your article was high in the search results.*

It certainly scared me off when I first investigated CQS.  My `Repository` based generic solution had just a few classes.  Why go backwards, it made no sense.

This is an attempt to make it succinct!  I now use it over my generic repository code.

## Basic Interfaces and Classes

The basic methodology can be defined by two interfaces.

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

To demonstrate what I mean by "verbose", here's a classic implementation to add a `WeatherForecast` record.

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

Relatively small classes, but you need 3 sets (one each for add, update and delete) for each entity. That's 3 times *x* entities just for the commands.  The same principles apply to queries.  **DRY** hung out to dry!

## A Succinct Implementation

To build a more succinct implementation we need to accept:

 - The 80/20 rule.  Not every request can be fulfilled by our standard implementation.  We need a custom approach for these.
 - We need a "compliant" generics based ORM to interface with our data store.  This implementation uses *Entity Framework* which gives us that. 
 - There's going to be some quite complicated generics implemented in the base classes.

## Results

Before diving into requests and handlers, we need a set of standard results they return: `TResult` of the request.  Each is defined a `struct` containing status information and, if a request, the data set.  They're self explanatory.

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

`TRecord` represents data classes retrieved from the data store using the ORM.  It's qualified as a `class` and implements an empty constructor `new()`.

### Commands

All commands:

1. Take a record which we can define as `TRecord`.
2. Return an async `Task<CommandResult>` to fix `TResult`.

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

> Why implement these empty classes?  

We need a one-to-one relationship (requests -> handlers) so we need a handler for each type or command.

### The Handlers

There's no benefit in creating interfaces or base classes for handlers so we implement Create/Update/Delete commands as three separate classes.  `TRecord` defines the record class and `TDbContext` the `DbContext` used in the DI `DbContextFactory`.

We don't need the actual `DbContext` class: we use generics in `DbContext`.   `Set<TRecord>` method finds the `DbSet` instances of `TRecord` and `Update<TRecord>`, `Add<TRecord>` and `Delete<TRecord>` methods with `SaveChangesAsync` implement the commands. 

#### Add Handler

```csharp
public class AddRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<AddRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;
    protected readonly AddRecordCommand<TRecord> command;

    public AddRecordCommandHandler(IDbContextFactory<TDbContext> factory, AddRecordCommand<TRecord> command)
    {
        this.command = command;
        this.factory = factory;
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

#### Update Handler

```csharp
public class UpdateRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<UpdateRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;
    protected readonly UpdateRecordCommand<TRecord> command;

    public UpdateRecordCommandHandler(IDbContextFactory<TDbContext> factory, UpdateRecordCommand<TRecord> command)
    {
        this.command = command;
        this.factory = factory;
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

#### Delete Handler

```csharp
public class DeleteRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<DeleteRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;
    protected readonly IRecordCommand<TRecord> command;

    public DeleteRecordCommandHandler(IDbContextFactory<TDbContext> factory, IRecordCommand<TRecord> command)
    {
        this.command = command;
        this.factory = factory;
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

## The Generic Factory Broker

We can now define a factory interface and class to abstract the execution of *Requests* against their respective *Handlers*.  Personal: I always call these Core -> Data domain interfaces *Brokers*.

The interface:

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
The server implementation, which maps the request to the associated handler.

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

We can now call `ExecuteAsync` on the broker, setting `TRecord` and passing it the relevant command and it will execute the associated handler.

```
var command = new UpdateRecordCommand<DboWeatherForecast>(editedRecord);
var result = await broker.ExecuteAsync(command);
```

### Queries

Queries aren't quite so uniform.

1. There are various types of `TResult`.
2. They have specific *Where* and *OrderBy* requirements.

We define three Query requests:

### RecordQuery

This returns a `RecordProviderResult` containing a single record based on a provided Id.

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

This returns a `FkListProviderResult` containing an `IEnumerable` of `IFkListItem`.  `FkListItem` is a simple object containing a *Guid/Name* pair.  It's principle use is in foreign key *Select* controls in the UI.



```csharp
public record FKListQuery<TRecord>
    : ICQSRequest<ValueTask<FKListProviderResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
}
```

## Handlers

The corresponding query handlers are:

### RecordQueryHandler

Creatinge a "generic" version can be challenging depending on the ORM.

The key concepts to note are:

1. The use of *unit of work* `DbContexts` from the `IDbContextFactory`.
2. `_dbContext.Set<TRecord>()` gets the `DbSet` for `TRecord`.
3. The use of two methodologies to apply the query.  

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
    {
        var _dbContext = _factory.CreateDbContext();

        TRecord? record = null;

        // first check if the record implements IRecord.  If so we can do a cast and then do the quesry via the Id property directly 
        if ((new TRecord()) is IRecord)
            record = await _dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Id == _query.RecordId);

        // Try and use the EF FindAsync implementation
        if (record == null)
            record = await _dbContext.FindAsync<TRecord>(_query.RecordId);

        if (record is null)
        {
            _message = "No record retrieved";
            _success = false;
        }

        return new RecordProviderResult<TRecord>(record, _success, _message);
    }
}
```

### RecordListQueryHandler

The key concepts to note here are:

1. The use of *unit of work* `DbContexts` from the `IDbContextFactory`.
2. `_dbContext.Set<TRecord>()` to get the `DbSet` for `TRecord`.
3. The use of `IQueryable` to build queries.

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
        dbSet = this.GetCustomQueries(dbSet);

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

        IQueryable<TRecord> dbSet = dbContext.Set<TRecord>();
        dbSet = this.GetCustomQueries(dbSet);

        count = await dbSet.CountAsync();
        return true;
    }

    protected virtual IQueryable<TRecord> GetCustomQueries(IQueryable<TRecord> query)
        => query;
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

## The Generic Factory Broker Methods

Havikng defined our Requests/Handlers in one-to-one relationships we can now define the factory methods to abstract the execution of *Requests* against their respective *Handlers*.

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

The solution defines a set of simple XUnit based tests to test the pipeline.  These build out a DI service container rather than use Mock. 

Note:

1. WeatherTestDataProvider is a singleton class that provides a set of test data.
2. `GetServiceProvider` builds out the DI container with the necessary DI classes loaded.

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

## Custom Requests

How do we implement custom requests?

Here's a simple example.

### WeatherForecasts By Summary

We need an `IEnumerable` collection of `DvoWeatherForcast` filtered by `Summary`.

The Request query looks like this.  Note it inherits from our `RecordListQuery` with an added `WeatherSummaryId` property.

```csharp
public record WeatherForecastBySummaryListQuery
    : RecordListQuery<DvoWeatherForecast>
{
    public readonly Guid? WeatherSummaryId;

    public WeatherForecastBySummaryListQuery(Guid? weatherSummaryId, ListProviderRequest request)
        :base(request)
    { 
        WeatherSummaryId = weatherSummaryId;
        Request = request;
    }
}
```

And the Handler extends the `RecordListQueryHandler` by defining an overridden `GetCustomQueries` method that adds the `WeatherForecastId` filter to the `IQuerable` query.

```csharp
public class WeatherForecastBySummaryListQueryHandler<TDbContext>
    :RecordListQueryHandler<DvoWeatherForecast, TDbContext>
    where TDbContext : DbContext
{
    private WeatherForecastBySummaryListQuery _customQuery;

    public WeatherForecastBySummaryListQueryHandler(IDbContextFactory<TDbContext> factory, WeatherForecastBySummaryListQuery query)
        : base(factory, query)
        => _customQuery = query;

    protected override IQueryable<DvoWeatherForecast> GetCustomQueries(IQueryable<DvoWeatherForecast> query)
    {
        query = query.Where(item => item.WeatherSummaryId == _customQuery.WeatherSummaryId);
        return query;
    }
}
```

### Factory

How we implement the factory depends on the project size and how many custom queries we have.  On a small project with relatively few custom requests, implement a single custom factory.

```csharp
public interface ICustomCQSDataBroker
{
    public ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastBySummaryListQuery query);
}
```

```csharp
public class ServerCustomCQSDataBroker<TDbContext>
    :ICustomCQSDataBroker
    where TDbContext : DbContext
{
    protected readonly IDbContextFactory<TDbContext> factory;

    public ServerCustomCQSDataBroker(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(WeatherForecastBySummaryListQuery query)
    {
        var handler = new WeatherForecastBySummaryListQueryHandler<TDbContext>(factory, query);
        return await handler.ExecuteAsync();
    }
}
```

## Summary

Hopefully I've demonstrated that you don't need to write huge numbers of request and handler classes to implement CQS principles in your data pipeline.  I'm open to improvements that can be made to this framework - it's only at version 2!


## Appendix

## Test Data Set

Before going any further I want to introduce my test data set.  As this pipeline was designed as part of a Blazor project, the weather is the setting.

There are two primary "tables":

```csharp
public record DboWeatherSummary 
{
    [Key] public Guid WeatherSummaryId { get; init; } = Guid.Empty;
    public string Summary { get; init; } = string.Empty
}
```

```csharp
public record DboWeatherForecast 
{
    [Key] public Guid WeatherForecastId { get; init; } = Guid.Empty;
    public Guid WeatherSummaryId { get; init; } = Guid.Empty;
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
}
```

One data "View":

```csharp
public record DvoWeatherForecast : IRecord
{
    [Key] public Guid Id { get; init; }
    public Guid WeatherSummaryId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
    public string Summary { get; init; } = String.Empty;
}
```

And one Foreign Key "View":

```csharp
public record FkWeatherSummaryId : BaseFkListItem { }
```

The underlying interface and base class for all Foreign key fields:

```csharp
public interface IFkListItem
{
    [Key] public Guid Id { get; }
    public string Name { get; }
}

public record BaseFkListItem : IFkListItem
{
    [Key] public Guid Id { get; init; }
    public string Name { get; init; } = String.Empty;
}
```

Note:

1. All are immutable records.
2. The `Key` attribute is used to label the primary key.
3. Guids are used as Id fields.
4. `TemperatureF` has gone.  It's an internal calculated parameter.  We'll add it back in the business logic.
5. `Dbo` records map to database table objects.
6. `Dvo` records map to database view objects.

There's a `WeatherTestDataProvider` singleton object to generate and access a test data set.


