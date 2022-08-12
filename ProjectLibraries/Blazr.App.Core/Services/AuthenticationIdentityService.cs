/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

//TODO - need Blazr.Core Itnerface
public class AuthenticationIdentityService
{
    public ClaimsPrincipal? User = null;

    public Guid Uid { get; private set; }

    private AuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationIdentityService(AuthenticationStateProvider auth)
    {
        _authenticationStateProvider = auth;
        _authenticationStateProvider.AuthenticationStateChanged += this.AuthStateChanged;
    }

    public async ValueTask GetUser()
        => await this.GetUser(_authenticationStateProvider.GetAuthenticationStateAsync());

    public async ValueTask GetUser(Task<AuthenticationState> task)
    {
        var state = await task;
        User = state.User;
        this.Uid = User.GetIdentityId();
    }

    public async void AuthStateChanged(Task<AuthenticationState> task)
        => await this.GetUser(task);

    public AuthenticationHeaderValue GetAPIAuthenticationHeader()
    {
        return new AuthenticationHeaderValue("BlazrAuth", this.GetAuthToken());
    }

    public string GetAuthToken()
    {
        var bytes = Encoding.UTF8.GetBytes(this.Uid.ToString());
        return Convert.ToBase64String(bytes);
    }

    public void Dispose()
        => _authenticationStateProvider.AuthenticationStateChanged -= this.AuthStateChanged;
}