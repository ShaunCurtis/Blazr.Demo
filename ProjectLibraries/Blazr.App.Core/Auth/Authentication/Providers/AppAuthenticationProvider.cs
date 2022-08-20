/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class AppAuthenticationStateProvider : AuthenticationStateProvider
{
    private IIdentityService _identityService;
    private Guid _userId = Guid.Empty;

    public AppAuthenticationStateProvider(IIdentityService identityService)
        => _identityService = identityService;

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await _identityService.GetIdentityAsync(_userId);
        return new AuthenticationState(result.Identity);
    }

    public Task<AuthenticationState> ChangeUser(Guid userId)
    {
        _userId = userId;
        var task = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(task);
        return task;
    }
}
