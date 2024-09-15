namespace Blazr.App.Core;

public static class GuidExtensions
{
    public static string ToDisplayId(this Guid value)
        => value.ToString().Substring(24, 8);
}
