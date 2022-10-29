/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public readonly struct ValidationResult
{
    public ValidationMessageCollection ValidationMessages { get; init; }
    
    public bool IsValid { get; init; }

    public bool ValidationNotRun { get; init; }
}
