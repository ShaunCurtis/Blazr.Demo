/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Security.Claims;

namespace Blazr.Core;

public interface IIdentityProvider
{
    public string? GetHttpSecurityHeader();
}
