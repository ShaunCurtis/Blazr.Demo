/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record RecordCountProviderResult
{
    public int Count { get; init; }
     
    public bool Success { get; init; }

    public string? Message { get; init; }

    public RecordCountProviderResult() { }

    public RecordCountProviderResult(int count, bool success = true, string? message = null)
    {
        this.Count = count;
        this.Success = success;
        this.Message = message;
    }
}
