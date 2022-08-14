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

    public ClaimsPrincipal Identity { get; private set; } = new ClaimsPrincipal();

    public event EventHandler? IdentityChanged;

    public ServerIdentityService(IdentityCQSHandler<TDbContext> identityCQSHandler)
        => _identityCQSHandler = identityCQSHandler;

    public async ValueTask<IdentityQueryResult> GetIdentityAsync(Guid Uid)
    {
        var result = await _identityCQSHandler.ExecuteAsync(new IdentityQuery { IdentityId = Uid });
        if (result.Success)
        {
            this.Identity = result.Identity;
            IdentityChanged?.Invoke(this, EventArgs.Empty);
        }
        return result;
    }
}
