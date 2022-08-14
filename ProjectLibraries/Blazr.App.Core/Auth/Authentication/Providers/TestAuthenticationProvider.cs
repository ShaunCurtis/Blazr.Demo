/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    private IIdentityService _identityService;

    public Guid UserId { get; private set; } = Guid.Empty;

    public TestAuthenticationStateProvider(IIdentityService identityService)
        => _identityService = identityService;

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await _identityService.GetIdentityAsync(UserId);
        return new AuthenticationState(result.Identity ?? new ClaimsPrincipal());
    }

    public Task<AuthenticationState> ChangeUser(Guid userId)
    {
        this.UserId = userId;
        var task = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(task);
        return task;
    }
}
