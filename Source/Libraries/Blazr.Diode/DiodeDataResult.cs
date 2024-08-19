/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public record DiodeResult
{
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static DiodeResult Success(string? message = null)
        => new DiodeResult() { Successful = true, Message = message };

    public static DiodeResult Failure(string message)
        => new DiodeResult() { Successful = false, Message = message };
}

public record DiodeMutationResult<T> : DiodeResult
        where T : class
{
    public T Item { get; init; } = default!;

    public static DiodeMutationResult<T> Success(T item, string? message = null)
        => new DiodeMutationResult<T>() { Successful = true, Item = item, Message = message };

    public static DiodeMutationResult<T> Failure(string message, T? item = null)
        => new DiodeMutationResult<T>() { Successful = false, Message = message, Item = item! };
}
