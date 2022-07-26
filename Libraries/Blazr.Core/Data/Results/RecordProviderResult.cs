/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record RecordProviderResult<TRecord>
{
    public TRecord? Record { get; init; }
     
    public bool Success { get; init; }

    public string? Message { get; init; }

    public RecordProviderResult() { }

    public RecordProviderResult(TRecord? record, bool success = true, string? message = null)
    {
        this.Record = record;
        this.Success = success;
        if (record is null)
            this.Success = false;
        
        this.Message = message;
    }
}
