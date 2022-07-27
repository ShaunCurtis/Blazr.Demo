/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

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
