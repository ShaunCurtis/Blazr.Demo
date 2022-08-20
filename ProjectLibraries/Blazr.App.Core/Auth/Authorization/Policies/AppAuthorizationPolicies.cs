/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public static class AppAuthorizationPolicies
{
    // Role constants
    public const string AdminRole = "AdminRole";
    public const string UserRole = "UserRole";
    public const string VisitorRole = "VisitorRole";

    //Policy Name constants
    public const string IsEditorPolicy = "IsEditorPolicy";
    public const string IsViewerPolicy = "IsViewerPolicy";
    public const string IsManagerPolicy = "IsManagerPolicy";
    public const string IsAdminPolicy = "IsAdminPolicy";
    public const string IsUserPolicy = "IsUserPolicy";
    public const string IsVisitorPolicy = "IsVisitorPolicy";

    public static class AuthorizationPolicies
    {
        public static AuthorizationPolicy IsAdminAuthorizationPolicy
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole(AdminRole)
            .Build();

        public static AuthorizationPolicy IsUserAuthorizationPolicy
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole(AdminRole, UserRole)
            .Build();

        public static AuthorizationPolicy IsVisitorAuthorizationPolicy
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole(AdminRole, UserRole, VisitorRole)
            .Build();

        public static AuthorizationPolicy IsEditorAuthorizationPolicy
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new RecordEditorAuthorizationRequirement())
            .Build();

        public static AuthorizationPolicy IsManagerAuthorizationPolicy
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new RecordManagerAuthorizationRequirement())
            .Build();

        public static AuthorizationPolicy IsViewerAuthorizationPolicy
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    }

    public static Dictionary<string, AuthorizationPolicy> Policies
    {
        get
        {
            var policies = new Dictionary<string, AuthorizationPolicy>();

            policies.Add(IsAdminPolicy, AuthorizationPolicies.IsAdminAuthorizationPolicy);
            policies.Add(IsUserPolicy, AuthorizationPolicies.IsUserAuthorizationPolicy);
            policies.Add(IsVisitorPolicy, AuthorizationPolicies.IsVisitorAuthorizationPolicy);

            policies.Add(IsManagerPolicy, AuthorizationPolicies.IsManagerAuthorizationPolicy);
            policies.Add(IsEditorPolicy, AuthorizationPolicies.IsEditorAuthorizationPolicy);
            policies.Add(IsViewerPolicy, AuthorizationPolicies.IsViewerAuthorizationPolicy);
            return policies;
        }
    }

    public static void AddAppAuthServices(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();

        // Add the Authorization Handlers
        services.AddSingleton<IAuthorizationHandler, RecordOwnerEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordManagerAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordOwnerManagerAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordOwnerAsUidEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordEditorAsUidAuthorizationHandler>();

        // Add Authorization and all the Policies
        services.AddAuthorizationCore(config =>
        {
            foreach (var policy in AppAuthorizationPolicies.Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });

    }
}
