/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Presentation;

public class RecordChangedEventArgs : EventArgs
{
    public object Record { get; init; } = new object();

    public static RecordChangedEventArgs Create(object record)
        => new RecordChangedEventArgs { Record = record };
}
