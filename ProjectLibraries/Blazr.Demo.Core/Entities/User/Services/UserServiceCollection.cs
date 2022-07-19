/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class UserServiceCollection
{
    public static void AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
    }
}
