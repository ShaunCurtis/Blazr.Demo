/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class ValidationState
{
    private bool _tripped;

    public bool IsValid => !_tripped;

    public bool IsInvalid => _tripped;

    public void Trip()
        => _tripped = true;

    public void TripOnTrue(bool trip)
    {
        if (trip)
            _tripped = true;
    }

    public void TripOnFalse(bool trip)
    {
        if (!trip)
            _tripped = true;
    }
}

