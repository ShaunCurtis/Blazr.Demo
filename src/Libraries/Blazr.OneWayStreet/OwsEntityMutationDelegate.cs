/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet;

/// <summary>
/// Delegate Declaration for a mutation
/// It defines a function that receives a TState object
/// And returns a new TState object with the mutation applied
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <param name="entity"></param>
/// <returns></returns>
public delegate Task<OwsMutationResult<TEntity>> OwsEntityMutationDelegate<TEntity>( OwsMutationRequest<TEntity> request) where TEntity : class, IOwsEntity;