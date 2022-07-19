/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    private UserService _userService;
    public Guid UserId { get; private set; } = Guid.Empty;

    public TestAuthenticationStateProvider(UserService userService)
        => _userService = userService;

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _userService.GetUserAsync(UserId);
        return new AuthenticationState(user);
    }

    public Task<AuthenticationState> ChangeUser(Guid userId)
    {
        this.UserId = userId;
        var task = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(task);
        return task;
    }
}
