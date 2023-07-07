/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Auth;

public class CustomAuthorizationRequirement : IAuthorizationRequirement { }

public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizationRequirement>
{
    // Demo to show you cn inject any service
    private readonly NavigationManager _navigationManager;

    public CustomAuthorizationHandler(NavigationManager navigationManager)
        => _navigationManager = navigationManager;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizationRequirement requirement)
    {
        // You can do this directly in the policy.  This is just a simple demo
        if (context.User.IsInRole("AdminRole"))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
