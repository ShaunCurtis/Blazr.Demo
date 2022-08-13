/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class AuthenticationIdentityService 
    : IAuthenticationIdentityService, IDisposable
{
    private AuthenticationStateProvider _authenticationStateProvider;

    public ClaimsPrincipal? Identity { get; private set; } = null;

    public Guid Uid { get; private set; }

    public AuthenticationIdentityService(AuthenticationStateProvider auth)
    {
        _authenticationStateProvider = auth;
        //_authenticationStateProvider.AuthenticationStateChanged += this.AuthStateChanged;
    }

    public async ValueTask GetUser()
        => await this.GetUser(_authenticationStateProvider.GetAuthenticationStateAsync());

    public async ValueTask GetUser(Task<AuthenticationState> task)
    {
        var state = await task;
        this.Identity = state.User;
        this.Uid = this.Identity.GetIdentityId();
    }

    private async void AuthStateChanged(Task<AuthenticationState> task)
        => await this.GetUser(task);

    public AuthenticationHeaderValue GetAPIAuthenticationHeader()
        => new AuthenticationHeaderValue("BlazrAuth", this.GetAuthToken());

    private string GetAuthToken()
    {
        var bytes = Encoding.UTF8.GetBytes(this.Uid.ToString());
        return Convert.ToBase64String(bytes);
    }

    public void Dispose()
        => _authenticationStateProvider.AuthenticationStateChanged -= this.AuthStateChanged;
}