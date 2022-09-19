/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record RecordProviderResult<TRecord>
{
    public TRecord? Record { get; init; } = default(TRecord?);

    public bool Success { get; init; } = false;

    public string Message { get; init; } = string.Empty;

    public RecordProviderResult() { }

    public static RecordProviderResult<TRecord> Failure(string message)
        => new RecordProviderResult<TRecord> { Message = message };

    public static RecordProviderResult<TRecord> Successful(TRecord record, string? message = null)
        => new RecordProviderResult<TRecord> { Record= record, Success = true, Message = message ?? "The query completed successfully" };
}
