/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class AuthServicesCollection
{
    public static void AddAppAuthServerServices(this IServiceCollection services)
    {
        services.AddAuthentication("BlazrAuth").AddScheme<AppAuthOptions, AppAuthenticationHandler>("BlazrAuth", null);

        services.AddScoped<IIdentityService, IdentityService>();
    }
}
