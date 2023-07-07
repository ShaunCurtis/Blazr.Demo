/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public record struct SortDefinition
{
    public string SortField { get; init; } = string.Empty;
    public bool SortDescending { get; init; }

    public SortDefinition( string sortField, bool sortDescending) 
    { 
        SortField = sortField;
        SortDescending = sortDescending;
    }
}
