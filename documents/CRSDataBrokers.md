# Command/Query Data Brokers

## The CQS Pattern

The CQS pattern is a little more complicated for simple implementations, but comes into it's own in more complex projects.  The principle is to serarate out Command activities from Query Activities and have a Query/Command object and a Handler object for each database activity.

In essence:
- The `HandlerRequest` defines any information the handler needs to execute the request and what it expects as a return value,
- The Handler executes whatever it needs to do to produce the result defined by `HandlerRequest` based on the information provided by the `HandlerRequest`.   

In concept, it's very simple, and relatively easy to implement.  The problem is that in most implementations that implement the based interfaces, there's a lot of code repetition.

To fix this problem requires some base abstract classes.  At this point it gets a little more complex in the base class boilerplate code, but the result is simple concise record based implementations.

We'll walk through an example to get a better understanding.

First the base interfaces.

## HandlerRequests

An `IHandlerRequest`: 
1. Defines the result that the query will return as `TResult`.
2. Defines a unique Id that can be used for logging and transactional purposes between front end and back end systems. 

```csharp
public interface IHandlerRequest<out TResult>
{
    public Guid TransactionId { get;}
}
```

## Handlers

A handler takes an `IHandlerRequest` and gets the `TResult`.  The core pattern looks like this.  `ExecuteAsync` runs the handler and outputs the defined result.

```csharp
public interface IRecordActionHandler<in TAction, TResult>
    where TAction : IRecordAction<TResult>
{
    ValueTask<TResult?> ExecuteAsync();
}
```

## A Classic Implementation

At the implementation level we can define an implementation :

```csharp
public class AddWeatherForecastCommandHandlerFull
    : IRequestHandler<AddWeatherForecastCommand, ValueTask<CommandResult>>
{
    protected readonly IWeatherDbContext dbContext;
    protected readonly AddWeatherForecastCommand command;

    public AddWeatherForecastCommandHandlerFull(IWeatherDbContext dbContext, AddWeatherForecastCommand command)
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

With `AddWeatherForecastCommand`:

```csharp
public class AddWeatherForecastCommand
    : IHandlerRequest<ValueTask<CommandResult>>
{
    public DboWeatherForecast Record { get; private set; } = default!;

    public AddWeatherForecastCommand(DboWeatherForecast record)
        => this.Record = record;
}
```

Relatively small classes, but think about implementing 3 sets (one each for add, update and delete) for each entity where we have *x* entities.  That's a lot of repetitive code.  The same principles apply to queries.

## Building Base Classes

### Commands

We can use a generic `TRecord` to represent `DboWeatherForecast`, and everything returns a `CommandResult` so we can fix `TResult`.

First an interface that implements `IHandlerRequest`.  It:

1. Fixes `TResult` as `ValueTask<CommandResult>`.
2. Defines a `TRecord` generic.
3. Defines a property `Record` of `TRecord`.  This will be the record we add/update/delete.

```csharp
public interface IRecordCommand<TRecord> 
    : IHandlerRequest<ValueTask<CommandResult>>
{
    public TRecord Record { get;}
}
```
Next we create an abstract implementation.

```csharp
public abstract class RecordCommandBase<TRecord>
     : IRecordCommand<TRecord>
{ 
    public TRecord Record { get; protected set; } = default!;

    public RecordCommandBase(TRecord record)
        => this.Record = record;
}
```

Our Weather Forecast entity implementation now just needs to fix `TRecord` to `DboWeatherForecast`.

```csharp
public class WeatherForecastCommand
    : RecordCommandBase<DboWeatherForecast>
{
    public WeatherForecastCommand(DboWeatherForecast record)
        : base(record) { }
}
```

A lot simpler and quicker to implement.

### The Handler

We create a base abstract implementation.  It defines `TRecord` to represent the record and implements `IRequestHandler`.  It's `New` requires the DbContext to execute the command against, and the `RecordCommand` for it's data.

```csharp
public abstract class RecordCommandHandlerBase<TAction, TRecord>
    : IRequestHandler<TAction, ValueTask<CommandResult>>
    where TAction : IHandlerRequest<ValueTask<CommandResult>>
    //IRecordCommand<TRecord>
{
    protected readonly IWeatherDbContext dbContext;
    protected readonly IRecordCommand<TRecord> command;

    public RecordCommandHandlerBase(IWeatherDbContext dbContext, IRecordCommand<TRecord> command)
    {
        this.command = command;
        this.dbContext = dbContext;
    }

    public async ValueTask<CommandResult> ExecuteAsync()
    {
        ExecuteCommand();
        return await dbContext.SaveChangesAsync() == 1
            ? new CommandResult(Guid.Empty, true, "Record Saved")
            : new CommandResult(Guid.Empty, false, "Error saving Record");
    }

    public abstract void ExecuteCommand();
}
```

We can use this base implementation to build a next level abstract class for create/add/delete operations.  Add looks like this.  We just fix the command to execute.  We can pass the `DbContext` a record and it will find the correct `DbSet` and apply the change to the correct record,   

```csharp
public abstract class AddRecordCommandHandlerBase<TAction, TRecord>
    : RecordCommandHandlerBase<TAction, TRecord>
    where TAction : IRecordCommand<TRecord> 
{

    public AddRecordCommandHandlerBase(IWeatherDbContext dbContext, IRecordCommand<TRecord> command)
        : base(dbContext, command)
    { }

    public override void ExecuteCommand()
    {
        if (command.Record is not null)
         this.dbContext.Add(this.command.Record);
    }
}
```

The final implementation just fixes the `TAction` command and the `IRecord`:

```csharp
public class AddWeatherForecastCommandHandler
    : AddRecordCommandHandlerBase<WeatherForecastCommand, DboWeatherForecast>
{
    public AddWeatherForecastCommandHandler(IWeatherDbContext dbContext, IRecordCommand<DboWeatherForecast> command)
        : base(dbContext,command)
    {}
}
```

### Queries

Queries present a different challenge.  In the application they either return a record or a collection of records.  

### Record Queries

We can define a record query as follows.

1. We supply the Id - in our case a `Guid`
2. We return a `RecordProviderResult` based on the `TRecord` generic type wrapped in a `ValueTask`.

```csharp
public record RecordQuery<TRecord>
    : IHandlerRequest<ValueTask<RecordProviderResult<TRecord>>>
{
    public readonly Guid? RecordId;

    public RecordQuery(Guid? recordId)
        => this.RecordId = recordId;
}
```

### Record Query Handler 

We can now define an abstract class with all the boilerplate code.  The code uses the `Key` attribute of `TRecord` to get the correct property and then reflection to get the property value in the IQuery iteration. 

```csharp
public abstract class RecordQueryHandlerBase<TRecord>
    : IRequestHandler<RecordQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
    where TRecord : class, new()
{
    private readonly IWeatherDbContext _dbContext;
    private readonly RecordQuery<TRecord> _query;

    public RecordQueryHandlerBase(IWeatherDbContext dbContext, RecordQuery<TRecord> query)
    {
        _dbContext = dbContext;
        _query = query;
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<RecordProviderResult<TRecord>> _executeAsync()
    {
        TRecord? record = null;
        if (GetKeyProperty(out PropertyInfo? value) && value is not null)
        {
            record = await _dbContext.Set<TRecord>()
                .SingleOrDefaultAsync(item => GuidCompare(value.GetValue(item)));
        }
        return new RecordProviderResult<TRecord>(record);
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

        return value is not null;
    }
}
```

Our final implementation then looks like this:

```csharp
public class WeatherForecastQueryHandler
    : RecordQueryHandlerBase<DvoWeatherForecast>
{
    public WeatherForecastQueryHandler(IWeatherDbContext dbContext, RecordQuery<DvoWeatherForecast> query)
        : base(dbContext, query) { }
}
```
### Record Collection Queries

We can define a record collection query as follows.

1. Defines a `ListProviderRequest`.
2. Returns a `ListProviderResult` based on the `TRecord` generic type wrapped in a `ValueTask`.
3. It's not abstract.

```csharp
public record RecordListQuery<TRecord>
    : IHandlerRequest<ValueTask<ListProviderResult<TRecord>>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    public ListProviderRequest Query { get; }
}
```

```csharp
public record WeatherForecastListQuery
    :RecordListQuery<DvoWeatherForecast>
{
    public readonly ListProviderRequest Query;
    public readonly Guid? DboWeatherLocationId;

    public WeatherForecastListQuery(Guid? weatherLocationId, ListProviderRequest query)
    { 
        DboWeatherLocationId = weatherLocationId;
        Query = query;
    }
}
```

### Record Collection Query Handler 

We can now define an abstract class with all the boilerplate code.  The code calls teo abstract classes that need to be implemented to get the paged record set and the count of the record set. 

```csharp
public abstract class RecordListQueryHandlerBase<TAction, TRecord>
    : IRequestHandler<TAction, ValueTask<ListProviderResult<TRecord>>>
    where TAction : IHandlerRequest<ValueTask<ListProviderResult<TRecord>>>
{
    protected IEnumerable<TRecord> items = new List<TRecord>();
    protected int count;

    protected readonly IWeatherDbContext dbContext;
    protected readonly RecordListQuery<TRecord> listQuery;

    public RecordListQueryHandlerBase(IWeatherDbContext dbContext, RecordListQuery<TRecord> query)
    {
        this.dbContext = dbContext;
        this.listQuery = query;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<ListProviderResult<TRecord>> _executeAsync()
    {
        if (this.listQuery is null)
            return new ListProviderResult<TRecord>(new List<TRecord>(), 0);

        this.count = await this.GetCountAsync();
        this.items = await this.GetItemsAsync();
        return new ListProviderResult<TRecord>(this.items, this.count);
    }

    protected abstract ValueTask<IEnumerable<TRecord>> GetItemsAsync();

    protected abstract ValueTask<int> GetCountAsync();
}
```

Our final implementation then looks like this.  It's the most complex because each query is in general unique.  Note the code leverages the `IQueryable` features to build a query before final execution to build the returned `IEnumerable` recod set.

```csharp
public class WeatherForecastListQueryHandler
    : RecordListQueryHandlerBase<WeatherForecastListQuery, DvoWeatherForecast>
{
    private WeatherForecastListQuery? _listquery => this.listQuery as WeatherForecastListQuery;

    public WeatherForecastListQueryHandler(IWeatherDbContext dbContext, WeatherForecastListQuery query)
        : base(dbContext, query)
    {}

    protected async override ValueTask<IEnumerable<DvoWeatherForecast>> GetItemsAsync()
    {
        if (_listquery is null)
            return new List<DvoWeatherForecast>();

        IQueryable<DvoWeatherForecast> dbSet = this.dbContext.DvoWeatherForecast;

        if (_listquery.DboWeatherLocationId is not null)
            dbSet = dbSet.Where(item => item.WeatherLocationId == _listquery.DboWeatherLocationId);

        if (_listquery.Query.Count > 0)
            dbSet = dbSet
                .Skip(_listquery.Query.StartIndex)
                .Take(_listquery.Query.Count);

        return await dbSet.ToListAsync();
    }
    
    protected async override ValueTask<int> GetCountAsync()
    {
        if (_listquery is null)
            return 0;

        IQueryable<DvoWeatherForecast> dbSet = this.dbContext.DvoWeatherForecast;

        if (_listquery.DboWeatherLocationId is not null)
            dbSet = dbSet.Where(item => item.WeatherLocationId == _listquery.DboWeatherLocationId);

        return await dbSet.CountAsync();
    }
}
```
