﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IAuthenticationIdentityService
{
    public ClaimsPrincipal? Identity { get; }

    public AuthenticationHeaderValue GetAPIAuthenticationHeader();
}