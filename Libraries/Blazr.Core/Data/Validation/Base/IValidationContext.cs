/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;
public interface IValidationContext
{
    public event EventHandler<ValidationStateEventArgs>? ValidationStateUpdated;
    public bool HasMessages(FieldReference field);
    public IEnumerable<string> GetMessages(FieldReference field);
    public ValidationResult Validate(FieldReference? field);
}
