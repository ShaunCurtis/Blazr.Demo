/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Edit;

public class RecordContextNotLoadedException : Exception
{
    public RecordContextNotLoadedException() { }

    public RecordContextNotLoadedException(string message): base(message) { }

    public RecordContextNotLoadedException(string message, Exception inner) : base(message, inner) { }

    public static RecordContextNotLoadedException Create(string message)
        => new(message);
}
