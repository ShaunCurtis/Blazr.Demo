/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public readonly struct DiodeState : IEquatable<DiodeState>
{
    public int Index { get; private init; }
    public string Value { get; private init; }

    internal DiodeState(int index, string value)
    {
        Index = index;
        Value = value; 
    }

    public static DiodeState Clean = new DiodeState(0, "Clean");
    public static DiodeState New = new DiodeState(1, "New");
    public static DiodeState Modified = new DiodeState(2, "Modified");
    public static DiodeState Deleted = new DiodeState(-1, "Deleted");

    public static bool operator ==(DiodeState lhs, DiodeState rhs)
        => lhs.Index == rhs.Index;

    public static bool operator !=(DiodeState lhs, DiodeState rhs)
        => lhs.Index != rhs.Index;

    public override bool Equals(object? obj)
        =>  obj is DiodeState other && this.Index == other.Index;

    public bool Equals(DiodeState other)
        => this.Index == other.Index;

    public override int GetHashCode()
        => this.Index.GetHashCode();
}
