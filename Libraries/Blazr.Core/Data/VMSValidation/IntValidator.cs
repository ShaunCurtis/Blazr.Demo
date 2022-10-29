/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class IntValidator : Validator<int>
{
    public IntValidator(int value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message) 
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public IntValidator LessThan(int test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value < test,
            message: message);

        return this;
    }

    public IntValidator GreaterThan(int test, string? message = null)
    {
        this.FailIfFalse(
            test: this.value > test,
            message: message);

        return this;
    }
}

public static class IntVMSValidatorExtensions
{
    public static IntValidator Validation(this int value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new IntValidator(value, fieldName, model, validationMessageStore, validationState, message);
}

