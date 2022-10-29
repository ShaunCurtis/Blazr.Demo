/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class ValidationStateEventArgs : EventArgs
{
    public bool ValidationState { get; set; }

    public string? Field { get; set; }

    public static ValidationStateEventArgs Create(bool state, string? field)
        => new ValidationStateEventArgs { ValidationState = state, Field = field };
}

