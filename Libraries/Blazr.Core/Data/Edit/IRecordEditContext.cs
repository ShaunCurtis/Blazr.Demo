/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core.Validation;

namespace Blazr.Core;

public interface IRecordEditContext<TRecord>
    where TRecord : class, new()
{
    public event EventHandler<ValidationStateEventArgs>? ValidationStateUpdated;
    public event EventHandler<string?>? FieldChanged;
    public Guid Uid { get; set; }

    public TRecord Record { get; }

    public bool IsDirty { get; }

    public bool IsNew { get; }

    public void Load(TRecord record);

    public TRecord CleanRecord { get; }

    public TRecord AsNewRecord();

    public void Reset();

    public bool HasMessages(string? fieldName = null);

    public IEnumerable<string> GetMessages(string? fieldName = null);

    public ValidationResult Validate(string? fieldName = null);
}