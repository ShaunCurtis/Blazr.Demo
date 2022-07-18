/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Auth.Simple.Core;

public class TestUserProvider
{
    public static Dictionary<Guid, ClaimsPrincipal> IdentityList = new Dictionary<Guid, ClaimsPrincipal>
    {
        { new Guid("00000000-0000-0000-0000-000000000001"), Admin },
        { new Guid("00000000-0000-0000-0000-000000000002"), User },
        { new Guid("00000000-0000-0000-0000-000000000003"), Visitor },
        { Guid.Empty, Anonymous },
    };

    public static Dictionary<Guid, string> UserList = new Dictionary<Guid, string>
    {
        { new Guid("00000000-0000-0000-0000-000000000001"), "Admin" },
        { new Guid("00000000-0000-0000-0000-000000000002"), "User" },
        { new Guid("00000000-0000-0000-0000-000000000003"), "Visitor" },
        { Guid.Empty, "Anonymous" },
    };

    public static ClaimsPrincipal Admin
    {
        get
        {
            var identity = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Sid, "00000000-0000-0000-0000-000000000001"),
                    new Claim(ClaimTypes.Name, "Administrator"),
                    new Claim(ClaimTypes.Role, "Admin")
                }, "Test authentication type");
            return new ClaimsPrincipal(identity);
        }
    }

    public static ClaimsPrincipal User
    {
        get
        {
            var identity = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Sid, "00000000-0000-0000-0000-000000000002"),
                    new Claim(ClaimTypes.Name, "Normal User"),
                    new Claim(ClaimTypes.Role, "User")
                }, "Test authentication type");
            return new ClaimsPrincipal(identity);
        }
    }

    public static ClaimsPrincipal Visitor
    {
        get
        {
            var identity = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Sid, "00000000-0000-0000-0000-000000000003"),
                    new Claim(ClaimTypes.Name, "Visitor"),
                    new Claim(ClaimTypes.Role, "Visitor")
                }, "Test authentication type");
            return new ClaimsPrincipal(identity);
        }
    }

    public static ClaimsPrincipal Anonymous
    {
        get
        {
            var identity = new ClaimsIdentity(new Claim[0], null);
            return new ClaimsPrincipal(identity);
        }
    }
}
