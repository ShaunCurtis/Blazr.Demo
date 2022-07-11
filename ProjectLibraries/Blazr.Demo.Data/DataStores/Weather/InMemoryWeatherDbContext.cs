/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

public class InMemoryWeatherDbContext
    : DbContext, IWeatherDbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecast { get; set; } = default!;

    public DbSet<DvoWeatherForecast> DvoWeatherForecast { get; set; } = default!;

    public DbSet<DboWeatherSummary> DboWeatherSummary { get; set; } = default!;

    public DbSet<DboWeatherLocation> DboWeatherLocation { get; set; } = default!;

    public DbSet<FkWeatherSummary> FkWeatherSummary { get; set; } = default!;

    public DbSet<FkWeatherLocation> FkWeatherLocation { get; set; } = default!;

    public InMemoryWeatherDbContext(DbContextOptions<InMemoryWeatherDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecast");
        modelBuilder.Entity<DboWeatherSummary>().ToTable("WeatherSummary");
        modelBuilder.Entity<DboWeatherLocation>().ToTable("WeatherLocation");

        modelBuilder.Entity<DvoWeatherForecast>()
            .ToInMemoryQuery(()
            => from f in this.DboWeatherForecast
               join s in this.DboWeatherSummary! on f.WeatherSummaryId equals s.WeatherSummaryId into fs
               from fsjoin in fs
               join l in this.DboWeatherLocation! on f.WeatherLocationId equals l.WeatherLocationId into fl
               from fljoin in fl
               select new DvoWeatherForecast
               {
                   Id = f.WeatherForecastId,
                   WeatherSummaryId = f.WeatherSummaryId,
                   WeatherLocationId = f.WeatherLocationId,
                   Date = f.Date,
                   Summary = fsjoin.Summary,
                   Location = fljoin.Location,
                   TemperatureC = f.TemperatureC,
               })
            .HasKey(x => x.Id);

        modelBuilder.Entity<FkWeatherSummary>()
            .ToInMemoryQuery(()
            => from s in this.DboWeatherSummary!
               select new FkWeatherSummary
               {
                   Id =s.WeatherSummaryId,
                   Name = s.Summary
               })
            .HasKey(x => x.Id);

        modelBuilder.Entity<FkWeatherLocation>()
            .ToInMemoryQuery(()
            => from l in this.DboWeatherLocation!
               select new FkWeatherLocation
               {
                   Id = l.WeatherLocationId,
                   Name = l.Location
               })
            .HasKey(x => x.Id);
    }
}
