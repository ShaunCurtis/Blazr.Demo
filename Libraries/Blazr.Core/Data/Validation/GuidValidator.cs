/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class GuidValidator : Validator<Guid>
{
    public GuidValidator(Guid value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message) 
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public GuidValidator NotEmpty(string? message = null)
    {
        this.FailIfTrue(
            test: this.value == Guid.Empty,
            message: message);

        return this;
    }
}

public static class GuidValidatorExtensions
{
    public static GuidValidator Validation(this Guid value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new GuidValidator(value, fieldName, model, validationMessageStore,validationState, message);
}

