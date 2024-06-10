/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed class InMemoryTestDbContext
    : DbContext
{
    public required DbSet<DboWeatherForecast> WeatherForecasts { get; set; } = default!;

    public InMemoryTestDbContext(DbContextOptions<InMemoryTestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecasts");
    }
}
