# Data

There are a multitude of patterns that can be used in data pipelines.  This solution implements two common ones:

1. A Generic Data Broker pattern.
2. A Command/Query Separation pattern. 

## Data Classes

Data classes are core domain objects: they are used throughout the application. The application defines three data classes to match the database objects required by the core domain and UI.

1. They are value based immutable `records`.
2. They have a new unique ID field labelled with the `[Key]` attribute for database compatibility.
3. `TemperatureF` has gone.  It's an internal calculated parameter.  We'll add it back in the business logic.
4. `Dbo` records map to database table objects.
5. `Dvo` records map to database view objects.
6. `GuidExtensions.Null` defines a specific Guid that represents null.  This can be used to test if the record is actually a null record.

```csharp
public record DboWeatherLocation 
{
    [Key]
    public Guid WeatherLocationId { get; init; } = GuidExtensions.Null;
    public string? Name { get; init; }
}
```

```csharp
public record DboWeatherForecast 
{
    [Key]
    public Guid WeatherForecastId { get; init; } = GuidExtensions.Null;
    public Guid WeatherLocationId { get; init; } = GuidExtensions.Null;
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
}
```

```csharp
public record DvoWeatherForecast 
{
    [Key]
    public Guid WeatherForecastId { get; init; }
    public Guid WeatherLocationId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
    public string? Summary { get; init; }
    public string? WeatherLocation { get; init; }
}
```

## DBContext

The application uses interfaces to abstract the context implementation from the data brokers.  The DI `DbContext` can be switched between test and production databases with no changes to code up the data pipeline.

### IWeatherDbContext

The interface exposes the `DbSet` properties that the data domain uses and any underlying `DbContext` methods and properties the data domain code needs to access.  In this instance surfacing `SaveChangesAsync` so the data brokers can run CRUD operations and commit them to the datastore.

```csharp
public interface IWeatherDbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecast { get; set; }
    public DbSet<DvoWeatherForecast> DvoWeatherForecast { get; set; }
    public DbSet<DboWeatherLocation> DboWeatherLocation { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### InMemoryDbContext

This is the implementation used while developing and testing the application.

```csharp
public class InMemoryWeatherDbContext 
    : DbContext, IWeatherDbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecast { get; set; } = default!;
    public DbSet<DvoWeatherForecast> DvoWeatherForecast { get; set; } = default!;
    public DbSet<DboWeatherLocation> DboWeatherLocation { get; set; } = default!;

    public InMemoryWeatherDbContext(DbContextOptions<InMemoryWeatherDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecast");
        modelBuilder.Entity<DboWeatherLocation>().ToTable("WeatherLocation");
        
        modelBuilder.Entity<DvoWeatherForecast>().ToInMemoryQuery(() 
            => from f in this.DboWeatherForecast
                join l in this.DboWeatherLocation!
                on f.WeatherLocationId equals l.WeatherLocationId
                select new DvoWeatherForecast
                {
                    WeatherForecastId = f.WeatherForecastId,
                    WeatherLocationId = l.WeatherLocationId,
                    Date = f.Date,
                    Summary = f.Summary,
                    TemperatureC = f.TemperatureC,
                    WeatherLocation = l.Name
                }
        );
    }
}
```

It inherits from `DbContext` and implements `IWeatherDbContext`.  It's designed to be used with an Entity Framework `In-Memory` database, so implements the `DvoWeatherForecast` view in the context as an `InMemoryQuery`.  This would point to a real view in a live database:

```csharp
modelBuilder.Entity<DvoWeatherForecast>().ToView("vw_WeatherForecasts");
```

## DBContextFactory

The application implements database access through an IDbContextFactory context and consumes DbContexts on a "unit of Work" principle.  This is important in applications implementing async data pipelines: more than one context may be in use at any one time.  See this [Microsoft article](https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#using-a-dbcontext-factory-eg-for-blazor) for further information.  The `DBContextFactory` is defined as a DI service as follows: 

```csharp
builder.Services.AddDbContextFactory<InMemoryDbContext>(options => options.UseInMemoryDatabase("TestWeatherDatabase"));
```
### Collections

The following general rules are applied to query methods that return collections and properties in classes in the application:

1. Collection properties are defined as `IEnumerable<T>` objects: not arrays or lists.

2. Never request unconstrained collections.  All query methods require an `ListProviderRequest`, a derived version of `ItemsProviderRequest`.  For those unfamiliar with `ItemsProviderRequest`, it's part of the list vitualization code, and contains the paging information.

```csharp
public struct ListProviderRequest
{
    public int StartIndex { get; }
    public int Count { get; }
    public CancellationToken CancellationToken { get; }
    public string? SortExpression { get; set; }
    public string? FilterExpression { get; set; }
    public ItemsProviderRequest Request => new (this.StartIndex, this.Count, this.CancellationToken);

    public ListProviderRequest(int startIndex, int count, CancellationToken cancellationToken, string? sortExpression = null, string? filterExpression = null )
    {
        StartIndex = startIndex;
        Count = count;
        CancellationToken = cancellationToken;
        SortExpression = sortExpression;
        FilterExpression = filterExpression;
    }

    public ListProviderRequest(ItemsProviderRequest request)
    {
        StartIndex = request.StartIndex;
        Count = request.Count;
        CancellationToken = request.CancellationToken;
        SortExpression = null;
        FilterExpression = null;
    }
}
```

3. Return `ListProviderResult` objects.  These are derived from the `ItemsProviderResult` object with additional status information

```csharp
public readonly struct ListProviderResult<TItem>
{
    public IEnumerable<TItem> Items { get; }
    public int TotalItemCount { get; }
    public bool Success { get; }
    public string? Message { get; }
    public ItemsProviderResult<TItem> ItemsProviderResult => new ItemsProviderResult<TItem>(this.Items, this.TotalItemCount);

    public ListProviderResult(IEnumerable<TItem> items, int totalItemCount, bool success = true, string? message = null)
    {
        Items = items;
        TotalItemCount = totalItemCount;
        Success = success;
        Message = message;
    }
}
```

## Return Objects

The application defines a set of return objects that all commands and queries return.

All commands only return status information in `CommandResult`:

```csharp
public struct CommandResult
{
    public Guid NewId { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; }

    public CommandResult( Guid newId, bool success, string message)
    {
        NewId = newId;
        Success = success;
        Message = message;
    }
}
```

All collection queries return a `ListProviderResult`:

```csharp
public readonly struct ListProviderResult<TItem>
{
    public IEnumerable<TItem> Items { get; }

    public int TotalItemCount { get; }

    public bool Success { get; }

    public string? Message { get; }

    public ItemsProviderResult<TItem> ItemsProviderResult => new ItemsProviderResult<TItem>(this.Items, this.TotalItemCount);

    public ListProviderResult(IEnumerable<TItem> items, int totalItemCount, bool success = true, string? message = null)
    {
        Items = items;
        TotalItemCount = totalItemCount;
        Success = success;
        Message = message;
    }
}
```

All record queries return a `RecordProviderResult`:

```csharp
public readonly struct RecordProviderResult<TRecord>
{
    public TRecord? Record { get; }
     
    public bool Success { get; }

    public string? Message { get; }

    public RecordProviderResult(TRecord? record, bool success = true, string? message = null)
    {
        Record = record;
        Success = success;
        if (record is null)
            Success = false;
        
        Message = message;
    }
}
```

## Test Data

We need to generate a data set for testing.  This is implemented through a "Singleton Pattern" class.

We can load the data sets into the database and refer to them directly in tests.

for those unsure about the *Singleton pattern*.  The class is code so that you can't create an instance of the class directly: the `New` method is private.  You must call the static method `WeatherTestData GetInstance()` to get the current instance.  It returns the current instance or creates one if one doesn't exists. 

```csharp
public class WeatherTestData
{
    private bool _initialized = false;
    private int RecordsToGenerate;
    private readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public IEnumerable<DboWeatherForecast> WeatherForecasts { get; private set; } = new List<DboWeatherForecast>();

    public IEnumerable<DboWeatherLocation> WeatherLocations { get; private set; } = new List<DboWeatherLocation>();

    private WeatherTestData()
    {

        // We need to populate this In Memory version of the database so we get a test data set from the static class WeatherForecastData
        if (!_initialized)
        {
            this.Load();
            _initialized = true;
        }
    }

    public void LoadDbContext(IDbContextFactory<InMemoryWeatherDbContext> factory)
    {
        using var dbContext = factory.CreateDbContext();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (dbContext.DboWeatherLocation.Count() == 0 || dbContext.DboWeatherForecast.Count() == 0)
        {
            dbContext.RemoveRange(dbContext.DboWeatherForecast.ToList());
            dbContext.RemoveRange(dbContext.DboWeatherLocation.ToList());
            dbContext.SaveChanges();
            dbContext.AddRange(this.WeatherLocations);
            dbContext.AddRange(this.WeatherForecasts);
            dbContext.SaveChanges();
        }
    }

    public void Load(int records = 100)
    {
        RecordsToGenerate = records;
        var forecasts = new List<DboWeatherForecast>();
        var locations = new List<DboWeatherLocation>();

        if (WeatherLocations.Count() == 0)
        {
            var weatherLocation = new DboWeatherLocation
            {
                WeatherLocationId = Guid.NewGuid(),
                Name = "Barcelona"
            };
            forecasts.AddRange(GetForecasts(weatherLocation.WeatherLocationId));
            locations.Add(weatherLocation);

            weatherLocation = new DboWeatherLocation
            {
                WeatherLocationId = Guid.NewGuid(),
                Name = "Alvor"
            };
            forecasts.AddRange(GetForecasts(weatherLocation.WeatherLocationId));
            locations.Add(weatherLocation);

            weatherLocation = new DboWeatherLocation
            {
                WeatherLocationId = Guid.NewGuid(),
                Name = "Rome"
            };
            forecasts.AddRange(GetForecasts(weatherLocation.WeatherLocationId));
            locations.Add(weatherLocation);

            WeatherForecasts = forecasts;
            WeatherLocations = locations;
        }
    }

    private IEnumerable<DboWeatherForecast> GetForecasts(Guid locationId)
    {
        return Enumerable.Range(1, RecordsToGenerate).Select(index => new DboWeatherForecast
        {
            WeatherForecastId = Guid.NewGuid(),
            WeatherLocationId = locationId,
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToList();
    }

    public DboWeatherForecast GetForecast()
    {
        var id = WeatherLocations.First().WeatherLocationId;
        return new DboWeatherForecast
        {
            WeatherForecastId = Guid.NewGuid(),
            WeatherLocationId = id,
            Date = DateTime.Now,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };
    }

    public DvoWeatherForecast? GetDvoWeatherForecast(Guid id)
    {
        var rec = WeatherForecasts.FirstOrDefault(item => item.WeatherForecastId == id);
        if (rec is not null)
        {
            var loc = WeatherLocations.FirstOrDefault(item => item.WeatherLocationId == rec.WeatherLocationId);
            if (loc is not null)
                return new DvoWeatherForecast
                {
                    WeatherForecastId = rec.WeatherForecastId,
                    WeatherLocationId = rec.WeatherLocationId,
                    WeatherLocation = loc?.Name ?? String.Empty,
                    Date = rec.Date,
                    TemperatureC = rec.TemperatureC,
                    Summary = rec.Summary
                };
        }
        return null;
    }

    public DboWeatherForecast? GetRandomRecord()
    {
        if (this.WeatherForecasts.Count() > 0)
        {
            var ran = new Random().Next(0, WeatherForecasts.Count());
            return this.WeatherForecasts.Skip(ran).FirstOrDefault();
        }
        return null;
    }

    private static WeatherTestData? _weatherTestData;

    public static WeatherTestData GetInstance()
    {
        if (_weatherTestData == null)
            _weatherTestData = new WeatherTestData();

        return _weatherTestData;
    }
}
```