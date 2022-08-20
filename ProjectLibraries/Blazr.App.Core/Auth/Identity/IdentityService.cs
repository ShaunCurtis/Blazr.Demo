/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class IdentityService
    : IIdentityService
{
    private IIdentityCQSHandler _identityCQSHandler;

    public ClaimsPrincipal Identity { get; private set; } = new ClaimsPrincipal();

    public event EventHandler? IdentityChanged;

    public IdentityService(IIdentityCQSHandler identityCQSHandler)
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
