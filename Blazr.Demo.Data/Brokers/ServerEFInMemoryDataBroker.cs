/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public class ServerEFInMemoryDataBroker<TDbContext>
    : ServerDataBroker<TDbContext>
    where TDbContext : DbContext
{
    public ServerEFInMemoryDataBroker(IDbContextFactory<TDbContext> db)
        : base(db)
            => WeatherTestDataProvider.Instance().LoadDbContext<TDbContext>(db);
}
