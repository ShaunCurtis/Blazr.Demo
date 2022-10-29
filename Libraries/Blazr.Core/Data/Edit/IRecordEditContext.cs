/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core.Validation;

namespace Blazr.Core;

public interface IRecordEditContext
{
    public event EventHandler<ValidationStateEventArgs>? ValidationStateUpdated;
    public event EventHandler<string?>? FieldChanged;

    public bool IsDirty { get; }

    public bool HasMessages(string? fieldName = null);

    public IEnumerable<string> GetMessages(string? fieldName = null);

    public ValidationResult Validate(string? fieldName = null);
}