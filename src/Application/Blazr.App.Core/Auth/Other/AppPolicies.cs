/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Auth;

public static class AuthRoles
{
    public const string AdminRole = "AdminRole";
    public const string UserRole = "UserRole";
    public const string VisitorRole = "VisitorRole";
}

public static class AuthPolicyNames
{
    public const string IsEditorPolicy = "IsEditorPolicy";
    public const string IsViewerPolicy = "IsViewerPolicy";
    public const string IsManagerPolicy = "IsManagerPolicy";
    public const string IsAdminPolicy = "IsAdminPolicy";
    public const string IsUserPolicy = "IsUserPolicy";
    public const string IsVisitor = "IsVisitor";
    public const string IsAdminAreaPolicy = "IsAdminAreaPolicy";
}

public static class AppPolicies
{
    public static AuthorizationPolicy IsAdminAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AuthRoles.AdminRole)
        .Build();

    public static AuthorizationPolicy IsUserAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AuthRoles.AdminRole, AuthRoles.UserRole)
        .Build();

    public static AuthorizationPolicy IsVisitorAuthorizationPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(AuthRoles.AdminRole, AuthRoles.UserRole, AuthRoles.VisitorRole)
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

    public static AuthorizationPolicy IsAdminAreaAuthPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new AdminAreaAuthorizationRequirement())
        .Build();

    public static Dictionary<string, AuthorizationPolicy> Policies = new Dictionary<string, AuthorizationPolicy>()
    {
        { AuthPolicyNames.IsAdminPolicy, IsAdminAuthorizationPolicy },
        { AuthPolicyNames.IsUserPolicy, IsUserAuthorizationPolicy},
        { AuthPolicyNames.IsVisitor, IsVisitorAuthorizationPolicy},
        { AuthPolicyNames.IsManagerPolicy, IsManagerAuthorizationPolicy},
        { AuthPolicyNames.IsEditorPolicy, IsEditorAuthorizationPolicy},
        { AuthPolicyNames.IsViewerPolicy, IsViewerAuthorizationPolicy},
        { AuthPolicyNames.IsAdminAreaPolicy, IsAdminAreaAuthPolicy},
    };

    public static void AddAppServerAuthServices(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, VerySimpleAuthenticationStateProvider>();
        //services.AddScoped<AuthenticationStateProvider, DumbAuthenticationStateProvider>();

        services.AddAppPolicyServices();
        services.AddAuthorization(config =>
        {
            foreach (var policy in Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });
    }

    public static void AddAppPolicyServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, RecordOwnerEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordEditorAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordManagerAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, RecordOwnerManagerAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, AdminAreaAuthorizationHandler>();
    }

    public static void AddAppAuthServices(this IServiceCollection services)
    {
        services.AddAuthorizationCore(config =>
        {
            foreach (var policy in Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });
    }

}

