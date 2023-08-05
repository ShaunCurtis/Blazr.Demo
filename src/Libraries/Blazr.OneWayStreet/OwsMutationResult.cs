/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet;

public record OwsMutationResult<TEntity> : IOwsResult<TEntity>
    where TEntity : class, IOwsEntity
{
    public TEntity? Entity { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static OwsMutationResult<TEntity> Success(TEntity entity, string? message = null)
        => new OwsMutationResult<TEntity>() { Successful = true, Message = message, Entity = entity };

    public static OwsMutationResult<TEntity> Failure(string message, TEntity? entity = null)
        => new OwsMutationResult<TEntity>() { Successful = false, Message = message, Entity = entity };
}