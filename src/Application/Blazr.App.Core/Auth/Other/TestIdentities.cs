/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core.Auth;

public static class TestIdentities
{
    public const string Provider = "Dumb Provider";

    public static ClaimsIdentity GetIdentity(string userName)
    {
        var identity = identities.FirstOrDefault(item => item.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (identity == null)
            return new ClaimsIdentity();

        return new ClaimsIdentity(identity.Claims, Provider);
    }

    private static List<TestIdentity> identities = new List<TestIdentity>()
        {
            Visitor1Identity,
            User1Identity,
            User2Identity,
            Admin1Identity,
        };

    public static List<string> GetTestIdentities()
    {
        var list = new List<string> { "None" };
        list.AddRange(identities.Select(identity => identity.Name!).ToList());
        return list;
    }

    public static Dictionary<Guid, string> TestIdentitiesDictionary()
    {
        var list = new Dictionary<Guid, string>();
        identities.ForEach(identity => list.Add(identity.Id, identity.Name));
        return list;
    }

    public static TestIdentity Visitor1Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-000000000001"),
            Name = "Visitor-1",
            Role = "VisitorRole"
        };

    public static TestIdentity User1Identity
        => new TestIdentity
        {
            Id = new Guid("20000000-0000-0000-0000-000000000001"),
            Name = "User-1",
            Role = "UserRole"
        };

    public static TestIdentity User2Identity
        => new TestIdentity
        {
            Id = new Guid("20000000-0000-0000-0000-000000000002"),
            Name = "User-2",
            Role = "UserRole"
        };

    public static TestIdentity Admin1Identity
        => new TestIdentity
        {
            Id = new Guid("30000000-0000-0000-0000-000000000001"),
            Name = "Admin-1",
            Role = "AdminRole"
        };
}

public record TestIdentity
{
    public string Name { get; set; } = string.Empty;

    public Guid Id { get; set; } = Guid.Empty;

    public string Role { get; set; } = string.Empty;

    public Claim[] Claims
        => new[]{
            new Claim(ClaimTypes.Sid, Id.ToString()),
            new Claim(ClaimTypes.Name, Name),
            new Claim(ClaimTypes.Role, Role)
    };

}

