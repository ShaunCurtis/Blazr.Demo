/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class RecordEditContextLoadException : Exception
{
    public RecordEditContextLoadException()
        : base($"A Record has already been loaded.  You can't overload it.") { }
    public RecordEditContextLoadException(string message)
        : base(message) { }
}
