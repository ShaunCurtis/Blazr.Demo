using System.Security.Claims;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

public class IdentityQueryHandler<TDbContext>
    : IIdentityQueryHandler
        where TDbContext : DbContext
{
    protected IDbContextFactory<TDbContext> factory;

    public IdentityQueryHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<IdentityRequestResult> ExecuteAsync(IdentityQuery query)
    {
        var dbContext = this.factory.CreateDbContext();
        IQueryable<DboIdentity> queryable = dbContext.Set<DboIdentity>();
        if (queryable is not null)
        {
            var record = await queryable.SingleOrDefaultAsync(item => item.Id == query.IdentityId, query.CancellationToken);
            if (record is not null)
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, record.Id.ToString()),
                    new Claim(ClaimTypes.Name, record.Name),
                    new Claim(ClaimTypes.Role, record.Role)
                }, "GuidIdentityProvider");
                return IdentityRequestResult.Successful(identity);
            }
            return IdentityRequestResult.Failure("No Identity exists.");
        }
        return IdentityRequestResult.Failure("No Identity Records Found.");
    }
}
