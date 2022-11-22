/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Edit;

public record FieldReference
{
    public Guid ObjectUid { get; init; } = Guid.Empty;
    public required string FieldName { get; init; }

    public static FieldReference Create(Guid objectUid, string fieldName)
        =>  new FieldReference { ObjectUid = objectUid, FieldName= fieldName };

    public static FieldReference Create(string fieldName)
        => new FieldReference { ObjectUid = Guid.Empty, FieldName = fieldName };
}
