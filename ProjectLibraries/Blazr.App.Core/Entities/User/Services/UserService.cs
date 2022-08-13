/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class UserService
    : BaseEntityService<UserEntity>
{
    //public readonly SortedDictionary<Guid, string> UserList = new SortedDictionary<Guid, string>();
    private IIdentityService _identityService;

    public Guid UserId { get; private set; }

    public UserService(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<ClaimsPrincipal> GetUserAsync(Guid Id)
    {
        this.UserId = Id;
        var result = await _identityService.GetIdentity(Id);
        if (result.Success && result.Identity is not null)
            return result.Identity;

        return new ClaimsPrincipal(new ClaimsIdentity(new Claim[0], null));
    }

    public AuthenticationHeaderValue GetAPIAuthenticationHeader()
        => new AuthenticationHeaderValue("BlazrAuth", this.GetAuthToken());

    private string GetAuthToken()      
        {
        var bytes = Encoding.UTF8.GetBytes(this.UserId.ToString());
        return Convert.ToBase64String(bytes);
    }
}
