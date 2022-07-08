/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
/// 
namespace Blazr.Demo.Data;

public interface IWeatherDbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecast { get; set; }

    public DbSet<DvoWeatherForecast> DvoWeatherForecast { get; set; }

    public DbSet<DboWeatherSummary> DboWeatherSummary { get; set; }

    public DbSet<DboWeatherLocation> DboWeatherLocation { get; set; }

    public DbSet<FkWeatherSummary> FkWeatherSummary { get; set; }

    public DbSet<FkWeatherLocation> FkWeatherLocation { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public EntityEntry Add(object entity);

    public EntityEntry Update(object entity);

    public EntityEntry Remove(object entity);

    public DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
