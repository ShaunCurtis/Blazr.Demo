/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class DvoMappingException : Exception
{
    public DvoMappingException()
        : base($"You can't map to a Dvo object.  All Dvo objects are readonly.") { }
    public DvoMappingException(string message)
        : base(message) { }
}
