/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct RecordProviderResult<TRecord>
{
    public TRecord? Record { get; }
     
    public bool Success { get; }

    public string? Message { get; }

    public RecordProviderResult(TRecord? record, bool success = true, string? message = null)
    {
        this.Record = record;
        this.Success = success;
        if (record is null)
            this.Success = false;
        
        this.Message = message;
    }
}
