/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet;

public record OwsMutationRequest<TEntity>(TEntity Entity, CancellationToken CancellationToken = new()) 
    where TEntity : class, IOwsEntity;
