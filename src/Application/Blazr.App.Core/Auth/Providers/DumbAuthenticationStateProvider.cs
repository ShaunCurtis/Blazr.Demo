/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Auth;

public class DumbAuthenticationStateProvider : AuthenticationStateProvider
{
    ClaimsPrincipal? _user => new ClaimsPrincipal(new ClaimsIdentity(UserIdentity.Claims, "Dumb Provider"));

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_user ?? new ClaimsPrincipal()));

    public static TestIdentity UserIdentity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-100000000002"),
            Name = "Dumb User",
            Role = "UserRole"
        };
}

