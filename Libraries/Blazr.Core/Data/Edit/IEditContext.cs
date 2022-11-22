/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core.Validation;

namespace Blazr.Core.Edit;

public interface IEditContext
{
    public event EventHandler<ValidationStateEventArgs>? ValidationStateUpdated;
    public event EventHandler<bool>? EditStateUpdated;
    public event EventHandler<string?>? FieldChanged;
    public Guid Uid { get; set; }

    public bool IsDirty { get; }

    public bool IsNew { get; }

    public bool HasMessages(FieldReference? field = null);
    public bool IsChanged(FieldReference field);

    public IEnumerable<string> GetMessages(FieldReference? field = null);

    public ValidationResult Validate(string? fieldName = null);
}