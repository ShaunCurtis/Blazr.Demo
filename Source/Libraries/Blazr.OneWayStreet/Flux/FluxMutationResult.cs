/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Flux;

public record FluxMutationResult<T> : IDataResult
        where T : class
{
    public T Item { get; init; } = default!;
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static FluxMutationResult<T> Success(T item, string? message = null)
        => new FluxMutationResult<T>() { Successful = true, Item = item, Message = message };

    public static FluxMutationResult<T> Failure(string message, T? item = null)
        => new FluxMutationResult<T>() { Successful = false, Message = message, Item = item! };
}
