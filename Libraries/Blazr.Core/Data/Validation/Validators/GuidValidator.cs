﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public class GuidValidator : ValidatorBase<Guid>
{
    public GuidValidator(Guid value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message)
        : base(value, fieldName, model, validationMessageStore, validationState, message) { }

    public GuidValidator(Guid value, Guid objectUid, string fieldName, ValidationMessageCollection validationMessages, ValidationState validationState, string? message)
    : base(value, objectUid, fieldName, validationMessages, validationState, message) { }

    public GuidValidator(Guid value, string? message = null)
    : base(value, message) { }

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
    public static GuidValidator Validation(this Guid value, string? message = null)
        => new GuidValidator(value, message);

    public static GuidValidator Validation(this Guid value, Guid objectUid, string fieldName, ValidationMessageCollection validationMessages, ValidationState validationState, string? message = null)
        => new GuidValidator(value, objectUid, fieldName, validationMessages, validationState, message);

    public static GuidValidator Validation(this Guid value, string fieldName, object model, ValidationMessageStore? validationMessageStore, ValidationState validationState, string? message = null)
        => new GuidValidator(value, fieldName, model, validationMessageStore, validationState, message);
}

