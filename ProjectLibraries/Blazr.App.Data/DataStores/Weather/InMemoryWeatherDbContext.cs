/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

public class InMemoryWeatherDbContext
    : DbContext
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
