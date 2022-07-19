/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class RecordManagerAuthorizationRequirement : IAuthorizationRequirement { }

public class RecordOwnerManagerAuthorizationHandler : AuthorizationHandler<RecordManagerAuthorizationRequirement, AppAuthFields>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordManagerAuthorizationRequirement requirement, AppAuthFields data)
    {
        var entityId = context.User.GetIdentityId();
        if (entityId != Guid.Empty && entityId == data.OwnerId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class RecordManagerAuthorizationHandler : AuthorizationHandler<RecordManagerAuthorizationRequirement, AppAuthFields>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordManagerAuthorizationRequirement requirement, AppAuthFields data)
    {
        if (context.User.IsInRole(AppAuthorizationPolicies.AdminRole))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
