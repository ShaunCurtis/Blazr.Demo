/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Edit;

public record FieldReference
{
    public Guid ObjectUid { get; init; }
    public string FieldName { get; init; }

    public FieldReference(Guid objectUid, string fieldName)
    {
        this.ObjectUid = objectUid;
        this.FieldName = fieldName;
    }

    public static FieldReference Create(Guid objectUid, string fieldName)
        => new(objectUid, fieldName);
}
