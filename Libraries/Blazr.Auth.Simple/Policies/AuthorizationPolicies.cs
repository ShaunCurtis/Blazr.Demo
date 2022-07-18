/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Auth.Simple;

public static class SimpleAuthorizationPolicies
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Visitor = "Visitor";

    public const string IsAdmin = "IsAdmin";
    public const string IsUser = "IsUser";
    public const string IsVisitor = "IsVisitor";

    public static AuthorizationPolicy IsAdminPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin")
        .Build();

    public static AuthorizationPolicy IsUserPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin", "User")
        .Build();

    public static AuthorizationPolicy IsVisitorPolicy
        => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin", "User", "Visitor")
        .Build();

    public static Dictionary<string, AuthorizationPolicy> Policies
    {
        get
        {
            var policies = new Dictionary<string, AuthorizationPolicy>();
            policies.Add(IsAdmin, IsAdminPolicy);
            policies.Add(IsUser, IsUserPolicy);
            policies.Add(IsVisitor, IsVisitorPolicy);
            return policies;
        }
    }
}
