/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

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
