/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public interface IMessageStoreValidation
{
    public bool Validate(ValidationMessageStore? validationMessageStore, string? fieldname, object? model = null);
}

