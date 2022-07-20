/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class RecordEditorAuthorizationRequirement : IAuthorizationRequirement { }

public class RecordOwnerEditorAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, AppAuthFields>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, AppAuthFields data)
    {
        var entityId = context.User.GetIdentityId();
        if (entityId != Guid.Empty && entityId == data.OwnerId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class RecordOwnerIdEditorAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, Guid>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, Guid id)
    {
        var entityId = context.User.GetIdentityId();
        if (entityId != Guid.Empty && entityId == id)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class RecordEditorAuthorizationHandler : AuthorizationHandler<RecordEditorAuthorizationRequirement, AppAuthFields>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RecordEditorAuthorizationRequirement requirement, AppAuthFields data)
    {
        if (context.User.IsInRole(AppAuthorizationPolicies.AdminRole))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
