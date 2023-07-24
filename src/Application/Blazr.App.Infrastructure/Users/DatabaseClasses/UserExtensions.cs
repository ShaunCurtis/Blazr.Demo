/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

internal static class UserExtensions
{
    internal static DboUser ToDbo(this User item)
        => new()
        {
            Uid = item.Uid.Value,
            UserName = item.UserName,
            Roles = item.Roles
        };

    internal static User FromDbo(this DboUser item)
        => new()
        {
            UserUid = new(item.Uid),
            UserName = item.UserName,
            Roles = item.Roles,
            EntityState = new(StateCodes.Existing),
        };
}
