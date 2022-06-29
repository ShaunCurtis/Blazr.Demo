/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct RecordCountProviderResult
{
    public int Count { get; }
     
    public bool Success { get; }

    public string? Message { get; }

    public RecordCountProviderResult(int count, bool success = true, string? message = null)
    {
        this.Count = count;
        this.Success = success;
        this.Message = message;
    }
}
