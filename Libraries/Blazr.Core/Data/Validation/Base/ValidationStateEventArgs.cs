/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class ValidationStateEventArgs : EventArgs
{
    public bool ValidationState { get; init; }
    public FieldReference? Field { get; init; }

    private ValidationStateEventArgs(bool validationState, FieldReference? field)
    {
        ValidationState = validationState;
        Field = field;
    }

    public static ValidationStateEventArgs Create(bool state, FieldReference? field)
        => new ValidationStateEventArgs(state, field );
}

