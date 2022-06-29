# Model Data, Data Stores and DBContexts

## Data Classes

Data classes are core domain objects: they are used throughout the application. The application defines three data classes to match the database objects required by the core domain and UI.

1. They are value based immutable `records`.
2. They have a new unique ID field labelled with the `[Key]` attribute for database compatibility.
3. `TemperatureF` has gone.  It's an internal calculated parameter.  We'll add it back in the business logic.
4. `Dbo` records map to database table objects.
5. `Dvo` records map to database view objects.
6. `GuidExtensions.Null` defines a specific Guid that represents null.  This can be used to test if the record is actually a null record.

```csharp
public record DboWeatherSummary 
{
    [Key] public Guid WeatherSummaryId { get; init; } = Guid.Empty;
    public string Summary { get; init; } = string.Empty; 
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

```csharp
public record DvoWeatherForecast 
{
    [Key] public Guid WeatherForecastId { get; init; }
    public Guid WeatherSummaryId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
    public string Summary { get; init; } = String.Empty;
}
```

Foreign key lists are implemented with an interface and base class.  FK lists are used in select controls in edit forms.

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

The Weather Summary FK list is then a simple class definition.

```csharp
public record FkWeatherSummaryId : BaseFkListItem { }
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
    public DbSet<DboWeatherSummary> DboWeatherSummary { get; set; }
    public DbSet<FkWeatherSummaryId> FkWeatherSummaryId { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public EntityEntry Add(object entity);
    public EntityEntry Update(object entity);
    public EntityEntry Remove(object entity);
    public DbSet<TEntity> Set<TEntity>() where TEntity : class;
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
    public DbSet<DboWeatherSummary> DboWeatherSummary { get; set; } = default!;
    public DbSet<FkWeatherSummaryId> FkWeatherSummaryId { get; set; } = default!;

    public InMemoryWeatherDbContext(DbContextOptions<InMemoryWeatherDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecast");
        modelBuilder.Entity<DboWeatherSummary>().ToTable("WeatherSummary");

        modelBuilder.Entity<DvoWeatherForecast>().ToInMemoryQuery(()
            => from f in this.DboWeatherForecast
               join s in this.DboWeatherSummary!
               on f.WeatherSummaryId equals s.WeatherSummaryId
               select new DvoWeatherForecast
               {
                   WeatherForecastId = f.WeatherForecastId,
                   WeatherSummaryId = f.WeatherSummaryId,
                   Date = f.Date,
                   Summary = s.Summary,
                   TemperatureC = f.TemperatureC,
               });

        modelBuilder.Entity<FkWeatherSummaryId>().ToInMemoryQuery(()
            => from s in this.DboWeatherSummary!
               select new FkWeatherSummaryId
               {
                   Id =s.WeatherSummaryId,
                   Name = s.Summary
               });
    }
}
```

It inherits from `DbContext` and implements `IWeatherDbContext`.  It's designed to be used with an Entity Framework `In-Memory` database, so implements the `DvoWeatherForecast` and `FkWeatherSummaryId` views in the context as an `InMemoryQuery`.  This would point to a real view in a live database:

```csharp
modelBuilder.Entity<DvoWeatherForecast>().ToView("vw_WeatherForecasts");
modelBuilder.Entity<FkWeatherSummaryId>().ToView("vw_WeatherSummmaryFkList");
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

## Return Objects

The application defines a set of return objects that all commands and queries return.

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

## Test Data

The solution needs a data set for testing.  This is implemented using a "Singleton Pattern" class.

Tests can load data sets into the database and refer to them through the DbContext or the test data instance.

For those unsure about the *Singleton pattern*.  The class is code so that you can't create an instance of the class directly: the `New` method is private.  You must call the static method `WeatherTestDataProvider.Instance()` to get the current instance.  It returns the current instance or creates one if one doesn't exists. 

```csharp
public class WeatherTestDataProvider
{
    private int RecordsToGenerate;

    public IEnumerable<DboWeatherForecast> WeatherForecasts { get; private set; } = Enumerable.Empty<DboWeatherForecast>();

    public IEnumerable<DboWeatherSummary> WeatherSummaries { get; private set; } = Enumerable.Empty<DboWeatherSummary>();

    private WeatherTestDataProvider()
        => this.Load();

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var weatherForcasts = dbContext.Set<DboWeatherForecast>();
        var weatherSummaries = dbContext.Set<DboWeatherSummary>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (WeatherSummaries.Count() == 0 || weatherForcasts.Count() == 0)
        {

            dbContext.RemoveRange(weatherSummaries.ToList());
            dbContext.RemoveRange(weatherForcasts.ToList());
            dbContext.SaveChanges();
            dbContext.AddRange(this.WeatherSummaries);
            dbContext.AddRange(this.WeatherForecasts);
            dbContext.SaveChanges();
        }
    }

    public void Load(int records = 100)
    {
        RecordsToGenerate = records;

        if (WeatherSummaries.Count() == 0)
        {
            this.LoadSummaries();
            this.LoadForecasts();
        }
    }

    private void LoadSummaries()
    {
        this.WeatherSummaries = new List<DboWeatherSummary> {
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Freezing"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Bracing"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Chilly"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Cool"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Mild"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Warm"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Balmy"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Hot"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Sweltering"},
            new DboWeatherSummary { WeatherSummaryId = Guid.NewGuid(), Summary = "Scorching"},
        };
    }

    private void LoadForecasts()
    {
        var summaryArray = this.WeatherSummaries.ToArray();

        this.WeatherForecasts = Enumerable.Range(1, RecordsToGenerate).Select(index => new DboWeatherForecast
        {
            WeatherForecastId = Guid.NewGuid(),
            WeatherSummaryId = summaryArray[Random.Shared.Next(summaryArray.Length)].WeatherSummaryId,
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
        }).ToList();
    }

    public DboWeatherForecast GetForecast()
    {
        var summaryArray = this.WeatherSummaries.ToArray();

        return new DboWeatherForecast
        {
            WeatherForecastId = Guid.NewGuid(),
            WeatherSummaryId = summaryArray[Random.Shared.Next(summaryArray.Length)].WeatherSummaryId,
            Date = DateTime.Now.AddDays(-1),
            TemperatureC = Random.Shared.Next(-20, 55),
        };
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

    public DvoWeatherForecast GetDvoWeatherForecast(DboWeatherForecast record)
    {
        return new DvoWeatherForecast
        {
            WeatherForecastId = record.WeatherForecastId,
            WeatherSummaryId = record.WeatherSummaryId,
            Date = record.Date,
            TemperatureC = record.TemperatureC,
            Summary = this.WeatherSummaries.SingleOrDefault(item => item.WeatherSummaryId == record.WeatherSummaryId)?.Summary ?? String.Empty
        };
    }

    private static WeatherTestDataProvider? _weatherTestData;

    public static WeatherTestDataProvider Instance()
    {
        if (_weatherTestData is null)
            _weatherTestData = new WeatherTestDataProvider();

        return _weatherTestData;
    }
}
```