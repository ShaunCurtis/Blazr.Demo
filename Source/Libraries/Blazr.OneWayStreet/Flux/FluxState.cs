/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Flux;

public record FluxState
{
    public int Index { get; private init; }
    public string Value { get; private init; }

    internal FluxState(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public CommandState AsCommandState
        => new(Index, Value);

    public static FluxState Clean = new FluxState(0, "Clean");
    public static FluxState New = new FluxState(1, "New");
    public static FluxState Modified = new FluxState(2, "Modified");
    public static FluxState Deleted = new FluxState(-1, "Deleted");
}
