/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetIdentityId(this ClaimsPrincipal principal)
    {
        if (principal is not null)
        {
            var claim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
            if (claim is not null && Guid.TryParse(claim.Value, out Guid id))
                return id;
        }
        return Guid.Empty;
    }
    public static Guid GetIdentityId(this ClaimsIdentity principal)
    {
        if (principal is not null)
        {
            var claim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
            if (claim is not null && Guid.TryParse(claim.Value, out Guid id))
                return id;
        }
        return Guid.Empty;
    }
}
