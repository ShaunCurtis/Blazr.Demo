/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class IdentityService
    : IIdentityService
{
    private IIdentityQueryHandler _identityQueryHandler;

    public ClaimsPrincipal Identity { get; private set; } = new ClaimsPrincipal();

    public event EventHandler? IdentityChanged;

    public IdentityService(IIdentityQueryHandler identityCQSHandler)
        => _identityQueryHandler = identityCQSHandler;

    public async ValueTask<IdentityRequestResult> GetIdentityAsync(Guid Uid)
    {
        var result = await _identityQueryHandler.ExecuteAsync(new IdentityQuery { IdentityId = Uid });
        if (result.Success)
        {
            this.Identity = new ClaimsPrincipal(result.Identity ?? new ClaimsIdentity());
            IdentityChanged?.Invoke(this, EventArgs.Empty);
        }
        return result;
    }
}
