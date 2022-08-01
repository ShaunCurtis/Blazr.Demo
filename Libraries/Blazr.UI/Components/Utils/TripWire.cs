/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class TripWire
{
    private bool _tripped = false;

    public bool IsTripped => _tripped;

    public bool TripOnTrue(bool isTrue)
    {
        _tripped = isTrue || _tripped;
        return isTrue;
    }

    public bool TripOnFalse(bool isFalse)
    {
        _tripped = !isFalse || _tripped;
        return !isFalse;
    }

    public bool Trip(bool isTrue)
    {
        _tripped = isTrue || _tripped;
        return isTrue;
    }

    public void Trip()
        => _tripped = true;

    public static TripWire Create(bool isTrue)
    {
        var wire = new TripWire();
        wire.Trip(isTrue);
        return wire;
    }
}
