/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Auth.Simple.Core;

public class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    public Guid UserId { get; private set; } = Guid.Empty;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = TestUserProvider.IdentityList[UserId];
        return Task.FromResult(new AuthenticationState(user));
    }

    public Task<AuthenticationState> ChangeUser(Guid userId)
    {
        this.UserId = userId;
        var task = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(task);
        return task;
    }
}
