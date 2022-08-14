/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

public class IdentityCQSHandler<TDbContext>
    : ICQSHandler<IdentityQuery, ValueTask<IdentityQueryResult>>, IIdentityCQSHandler
        where TDbContext : DbContext
{
    private IDbContextFactory<TDbContext> _factory;
    private IdentityQuery _query = default!;

    public IdentityCQSHandler(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<IdentityQueryResult> ExecuteAsync(IdentityQuery query)
    {
        if (query is not null)
        {
            var dbContext = _factory.CreateDbContext();
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var record = await dbContext.Set<DboUser>().SingleOrDefaultAsync(item => item.Id == query.IdentityId);

            if (record is not null)
            {
                var identity = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, record.Id.ToString()),
                    new Claim(ClaimTypes.Name, record.Name),
                    new Claim(ClaimTypes.Role, record.Role)
                }, "Test authentication type"));

                return new IdentityQueryResult { Identity = identity, Success = true };
            }
        }
        return new IdentityQueryResult {Success = false, Message = "No query defined" };
    }
}
