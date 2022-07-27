# Model Data, Data Stores and DBContexts

## Data Classes

Data classes (often knows as POCO classes) are core domain objects used throughout the application. The application defines three primary data classes to match the database objects required by the core domain and UI.

1. They are value based immutable `records`.
2. They have a new unique ID field labelled with the `[Key]` attribute for database compatibility.
3. `TemperatureF` has gone.  It's an internal calculated parameter.  We'll add it back in the business logic.
4. `Dbo` records map to database table objects.
5. `Dvo` records map to database view objects.
6. `GuidExtensions.Null` defines a specific Guid that represents null.  This can be used to test if the record is actually a null record.

```csharp
public record DboWeatherSummary 
    : IRecord
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;
    public string Summary { get; init; } = string.Empty
}
```
```csharp
public record DboWeatherLocation
    : IAuthRecord, IRecord
{
    [Key] public Guid Uid { get; init; }
    public Guid OwnerId { get; init; }
    public string Location { get; init; } = string.Empty;
}
```

```csharp
public record DboWeatherForecast 
    : IAuthRecord, IRecord
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;
    public Guid WeatherSummaryId { get; init; } = Guid.Empty;
    public Guid WeatherLocationId { get; init; }
    public Guid OwnerId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
}
```

```csharp
public record DvoWeatherForecast : IRecord
    : IRecord, IAuthRecord
{
    [Key] public Guid Uid { get; init; }
    public Guid WeatherSummaryId { get; init; }
    public Guid WeatherLocationId { get; init; }
    public Guid OwnerId { get; init; }
    public DateTimeOffset Date { get; init; }
    public int TemperatureC { get; init; }
    public string Summary { get; init; } = String.Empty;
    public string Location { get; init; } = String.Empty;
    public string Owner { get; init; } = String.Empty;
}
```

```csharp
public record DboUser
{
    [Key] public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = String.Empty;
    public string Role { get; init; } = String.Empty;
}
```

The `IRecord` interface is applied to any record that implements an "Uid" property which makes record lookups simple.  This looks like this:

```csherp
public interface IRecord 
{
    public Guid Uid { get; }
}
```

The `IAuthRecord` interface to any record that has an ownership field.  This is used to  authorize edit access on a record by record basis.

```csherp
public interface IAuthRecord 
{
    public Guid OwnerId { get; }
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

The Weather Summary and Weather Location FK lists are then a simple class definition.

```csharp
public record FkWeatherSummaryId : BaseFkListItem { }

public record FkWeatherLocation : BaseFkListItem { }
```

## DBContext

The application uses the generic methods in `DbContext` so there's no need for specific interfaces.

### InMemoryDbContext

This is the implementation used while developing and testing the application.

```csharp
public class InMemoryWeatherDbContext
    : DbContext, IWeatherDbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecast { get; set; } = default!;
    public DbSet<DvoWeatherForecast> DvoWeatherForecast { get; set; } = default!;
    public DbSet<DboWeatherSummary> DboWeatherSummary { get; set; } = default!;
    public DbSet<DboWeatherLocation> DboWeatherLocation { get; set; } = default!;
    public DbSet<DboUser> DboUser { get; set; } = default!;
    public DbSet<FkWeatherSummary> FkWeatherSummary { get; set; } = default!;
    public DbSet<FkWeatherLocation> FkWeatherLocation { get; set; } = default!;
    public InMemoryWeatherDbContext(DbContextOptions<InMemoryWeatherDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecast");
        modelBuilder.Entity<DboWeatherSummary>().ToTable("WeatherSummary");
        modelBuilder.Entity<DboWeatherLocation>().ToTable("WeatherLocation");
        modelBuilder.Entity<DboUser>().ToTable("User");

        modelBuilder.Entity<DvoWeatherForecast>()
            .ToInMemoryQuery(()
            => from f in this.DboWeatherForecast
               join s in this.DboWeatherSummary! on f.WeatherSummaryId equals s.Uid into fs
               from fsjoin in fs
               join l in this.DboWeatherLocation! on f.WeatherLocationId equals l.Uid into fl
               from fljoin in fl
               join u in this.DboUser! on f.OwnerId equals u.Id into fu
               from fujoin in fu
               select new DvoWeatherForecast
               {
                   Uid = f.Uid,
                   WeatherSummaryId = f.WeatherSummaryId,
                   WeatherLocationId = f.WeatherLocationId,
                   Date = f.Date,
                   Summary = fsjoin.Summary,
                   Location = fljoin.Location,
                   TemperatureC = f.TemperatureC,
                   OwnerId = f.OwnerId,
                   Owner = fujoin.Name
               })
            .HasKey(x => x.Uid);

        modelBuilder.Entity<FkWeatherSummary>()
            .ToInMemoryQuery(()
            => from s in this.DboWeatherSummary!
               select new FkWeatherSummary
               {
                   Id =s.Uid,
                   Name = s.Summary
               })
            .HasKey(x => x.Id);

        modelBuilder.Entity<FkWeatherLocation>()
            .ToInMemoryQuery(()
            => from l in this.DboWeatherLocation!
               select new FkWeatherLocation
               {
                   Id = l.Uid,
                   Name = l.Location
               })
            .HasKey(x => x.Id);
    }
}
```

It inherits from `DbContext`.  It's designed to be used with an Entity Framework `In-Memory` database, so implements views in the context as  `InMemoryQuery`c definitions.  This would point to a real view in a live database:

```csharp
modelBuilder.Entity<DvoWeatherForecast>().ToView("vw_WeatherForecasts");
modelBuilder.Entity<FkWeatherSummaryId>().ToView("vw_WeatherSummmaryFkList");
modelBuilder.Entity<FkWeatherLocationId>().ToView("vw_WeatherLocationFkList");
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

The application defines a set of return objects that all commands and queries return.  These are defined to be serializable - they have to be passed back though Web API calls.

```csharp
public record ListProviderResult<TRecord>
{
    public IEnumerable<TRecord> Items { get; init; }
    public int TotalItemCount { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }
    //....Constructors
}
```
```csaharp
public record RecordProviderResult<TRecord>
{
    public TRecord? Record { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }
    //....Constructors
}
```
```csaharp
public record CommandResult
{
    public Guid NewId { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; }
    //....Constructors
}
```
```csaharp
public record FKListProviderResult
{
    public IEnumerable<IFkListItem> Items { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }
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
    public IEnumerable<DboWeatherLocation> WeatherLocations { get; private set; } = Enumerable.Empty<DboWeatherLocation>();
    public IEnumerable<DboUser> Users { get; private set; } = Enumerable.Empty<DboUser>();

    private WeatherTestDataProvider()
        => this.Load();

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var weatherForcasts = dbContext.Set<DboWeatherForecast>();
        var weatherSummaries = dbContext.Set<DboWeatherSummary>();
        var weatherLocations = dbContext.Set<DboWeatherLocation>();
        var users = dbContext.Set<DboUser>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (weatherSummaries.Count() == 0 || weatherForcasts.Count() == 0)
        {

            dbContext.RemoveRange(weatherSummaries.ToList());
            dbContext.RemoveRange(weatherForcasts.ToList());
            dbContext.RemoveRange(weatherLocations.ToList());
            dbContext.RemoveRange(users.ToList());
            dbContext.SaveChanges();
            dbContext.AddRange(this.WeatherSummaries);
            dbContext.AddRange(this.WeatherForecasts);
            dbContext.AddRange(this.WeatherLocations);
            dbContext.AddRange(this.Users);
            dbContext.SaveChanges();
        }
    }

    public void Load(int records = 100)
    {
        RecordsToGenerate = records;

        if (WeatherSummaries.Count() == 0)
        {
            this.LoadLocations();
            this.LoadSummaries();
            this.LoadForecasts();
            this.LoadUsers();
        }
    }

    private void LoadSummaries()
    {
        this.WeatherSummaries = new List<DboWeatherSummary> {
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Freezing"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Bracing"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Chilly"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Cool"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Mild"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Warm"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Balmy"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Hot"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Sweltering"},
            new DboWeatherSummary { Uid = Guid.NewGuid(), Summary = "Scorching"},
        };
    }

    private void LoadLocations()
    {
        this.WeatherLocations = new List<DboWeatherLocation> {
            new DboWeatherLocation { Uid = Guid.NewGuid(), Location = "Gloucester", OwnerId = new Guid($"00000000-0000-0000-0000-100000000001") },
            new DboWeatherLocation { Uid = Guid.NewGuid(), Location = "Capestang", OwnerId = new Guid($"00000000-0000-0000-0000-100000000002")},
            new DboWeatherLocation { Uid = Guid.NewGuid(), Location = "Alvor", OwnerId = new Guid($"00000000-0000-0000-0000-100000000003")},
            new DboWeatherLocation { Uid = Guid.NewGuid(), Location = "Adelaide", OwnerId = new Guid($"00000000-0000-0000-0000-100000000003")},
        };
    }

    private void LoadForecasts()
    {
        var summaryArray = this.WeatherSummaries.ToArray();
        var forecasts = new List<DboWeatherForecast>();

        foreach (var location in WeatherLocations)
        {
            forecasts
                .AddRange(Enumerable
                    .Range(1, RecordsToGenerate)
                    .Select(index => new DboWeatherForecast
                    {
                        Uid = Guid.NewGuid(),
                        WeatherSummaryId = summaryArray[Random.Shared.Next(summaryArray.Length)].Uid,
                        WeatherLocationId = location.Uid,
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        OwnerId = location.OwnerId,
                    })
                );
        }

        this.WeatherForecasts = forecasts;
    }

    private void LoadUsers()
    {
        this.Users = new List<DboUser> {
            new DboUser { Id = GuidExtensions.Null, Name="Anonymous"},
            new DboUser { Id = new Guid("00000000-0000-0000-0000-000000000001"), Name="Visitor", Role= AppAuthorizationPolicies.VisitorRole},
            new DboUser { Id = new Guid("00000000-0000-0000-0000-100000000001"), Name="User-1", Role=AppAuthorizationPolicies.UserRole},
            new DboUser { Id = new Guid("00000000-0000-0000-0000-100000000002"), Name="User-2", Role=AppAuthorizationPolicies.UserRole},
            new DboUser { Id = new Guid("00000000-0000-0000-0000-100000000003"), Name="User-3", Role=AppAuthorizationPolicies.UserRole},
            new DboUser { Id = new Guid("00000000-0000-0000-0000-200000000001"), Name="Admin", Role=AppAuthorizationPolicies.AdminRole},
        };
    }

    public DboWeatherForecast GetForecast()
    {
        var summaryArray = this.WeatherSummaries.ToArray();

        return new DboWeatherForecast
        {
            Uid = Guid.NewGuid(),
            WeatherSummaryId = summaryArray[Random.Shared.Next(summaryArray.Length)].Uid,
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
            Uid = record.Uid,
            WeatherLocationId = record.WeatherLocationId,
            WeatherSummaryId = record.WeatherSummaryId,
            Date = record.Date,
            TemperatureC = record.TemperatureC,
            Summary = this.WeatherSummaries.SingleOrDefault(item => item.Uid == record.Uid)?.Summary ?? String.Empty
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