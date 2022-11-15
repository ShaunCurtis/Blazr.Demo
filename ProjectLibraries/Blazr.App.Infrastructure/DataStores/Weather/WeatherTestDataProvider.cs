/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

/// <summary>
/// A class to build a fixed data set for testing
/// The data varied but the count and Uids are always the same
/// </summary>
public class WeatherTestDataProvider
{
    private int RecordsToGenerate;

    public IEnumerable<DboWeatherForecast> WeatherForecasts { get; private set; } = Enumerable.Empty<DboWeatherForecast>();

    public IEnumerable<DboWeatherSummary> WeatherSummaries { get; private set; } = Enumerable.Empty<DboWeatherSummary>();

    public IEnumerable<DboWeatherLocation> WeatherLocations { get; private set; } = Enumerable.Empty<DboWeatherLocation>();

    public IEnumerable<DboIdentity> Identities { get; private set; } = Enumerable.Empty<DboIdentity>();

    private WeatherTestDataProvider()
        => this.Load();

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var weatherForcasts = dbContext.Set<DboWeatherForecast>();
        var weatherSummaries = dbContext.Set<DboWeatherSummary>();
        var weatherLocations = dbContext.Set<DboWeatherLocation>();
        var identities = dbContext.Set<DboIdentity>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (weatherSummaries.Count() == 0 || weatherForcasts.Count() == 0)
        {
            dbContext.RemoveRange(weatherSummaries.ToList());
            dbContext.RemoveRange(weatherForcasts.ToList());
            dbContext.RemoveRange(weatherLocations.ToList());
            dbContext.RemoveRange(identities.ToList());
            dbContext.SaveChanges();
            dbContext.AddRange(this.WeatherSummaries);
            dbContext.AddRange(this.WeatherForecasts);
            dbContext.AddRange(this.WeatherLocations);
            dbContext.AddRange(this.Identities);
            dbContext.SaveChanges();
        }
    }

    public void Load(int records = 100)
    {
        RecordsToGenerate = records;

        if (WeatherSummaries.Count() == 0)
            this.LoadSummaries();

        if (WeatherLocations.Count() == 0)
            this.LoadLocations();

        if (WeatherForecasts.Count() == 0)
            this.LoadForecasts();

        if (Identities.Count() == 0)
            this.LoadIdentities();
    }

    private void LoadSummaries()
    {
        this.WeatherSummaries = new List<DboWeatherSummary> {
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000001"), Summary = "Freezing"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000002"), Summary = "Bracing"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000003"), Summary = "Chilly"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000004"), Summary = "Cool"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000005"), Summary = "Mild"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000006"), Summary = "Warm"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000007"), Summary = "Balmy"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000008"), Summary = "Hot"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000009"), Summary = "Sweltering"},
            new DboWeatherSummary { Uid = new Guid("00000099-0000-0000-0000-000000000010"), Summary = "Scorching"},
        };
    }

    private void LoadLocations()
    {
        this.WeatherLocations = new List<DboWeatherLocation> {
            new DboWeatherLocation { Uid = new Guid("00000009-0000-0000-0000-000000000001"), Location = "Gloucester"},
            new DboWeatherLocation { Uid = new Guid("00000009-0000-0000-0000-000000000002"), Location = "Capestang"},
            new DboWeatherLocation { Uid = new Guid("00000009-0000-0000-0000-000000000003"), Location = "Alvor"},
            new DboWeatherLocation { Uid = new Guid("00000009-0000-0000-0000-000000000004"), Location = "Adelaide"},
        };
    }

    private void LoadIdentities()
    {
        this.Identities = new List<DboIdentity> {
            new DboIdentity { Id = Guid.Empty, Name="Anonymous"},
            new DboIdentity { Id = new Guid("00000000-0000-0000-0000-000000000001"), Name="Visitor", Role= "VisitorRole"},
            new DboIdentity { Id = new Guid("00000000-0000-0000-0000-100000000001"), Name="User-1", Role="UserRole"},
            new DboIdentity { Id = new Guid("00000000-0000-0000-0000-100000000002"), Name="User-2", Role="UserRole"},
            new DboIdentity { Id = new Guid("00000000-0000-0000-0000-100000000003"), Name="User-3", Role="UserRole"},
            new DboIdentity { Id = new Guid("00000000-0000-0000-0000-200000000001"), Name="Admin", Role="AdminRole"},
        };
    }

    private void LoadForecasts()
    {
        var summaryArray = this.WeatherSummaries.ToArray();
        var forecasts = new List<DboWeatherForecast>();

        int uniqueIndex = 0;
        foreach (var location in WeatherLocations)
        {
            for(var index = 0; index < RecordsToGenerate; index++)
            {
                uniqueIndex++;
                var rec = new DboWeatherForecast
                {
                    Uid = new Guid($"00000000-0000-0000-9999-{uniqueIndex.ToString("D12")}"),
                    WeatherSummaryId = summaryArray[Random.Shared.Next(summaryArray.Length)].Uid,
                    WeatherLocationId = location.Uid,
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                };
                forecasts.Add(rec);
            }
        }

        this.WeatherForecasts = forecasts;
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
        var record = new DboWeatherForecast();
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
            Location = this.WeatherLocations.SingleOrDefault(item => item.Uid == record.WeatherLocationId)?.Location ?? string.Empty,
            Summary = this.WeatherSummaries.SingleOrDefault(item => item.Uid == record.WeatherSummaryId)?.Summary ?? string.Empty
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
