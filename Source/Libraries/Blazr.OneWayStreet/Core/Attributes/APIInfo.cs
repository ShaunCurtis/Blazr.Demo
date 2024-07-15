/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class APIInfo : Attribute
{
    public string PathName { get; set; }
    public string ClientName { get; set; }

    public APIInfo(string pathName, string clientName)
    {
        PathName = pathName;
        ClientName = clientName;
    }
}
