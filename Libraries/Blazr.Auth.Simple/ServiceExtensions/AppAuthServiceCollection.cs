/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Auth.Simple;
using Blazr.Auth.Simple.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Auth;

public static class AppAuthServiceCollection
{
    public static void AddAppAuthServices(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, TestAuthenticationStateProvider>();
        services.AddAuthorizationCore(config =>
        {
            foreach (var policy in SimpleAuthorizationPolicies.Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });

    }
}
