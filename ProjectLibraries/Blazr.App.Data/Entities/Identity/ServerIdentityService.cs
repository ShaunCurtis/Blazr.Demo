/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Data;

public class ServerIdentityService<TDbContext>
    : IIdentityService
     where TDbContext : DbContext
{
    private IdentityCQSHandler<TDbContext> _identityCQSHandler;

    public ServerIdentityService(IdentityCQSHandler<TDbContext> identityCQSHandler)
        => _identityCQSHandler = identityCQSHandler;

    public async ValueTask<IdentityQueryResult> GetIdentity(Guid Uid)
        => await _identityCQSHandler.ExecuteAsync( new IdentityQuery { IdentityId = Uid });
}
