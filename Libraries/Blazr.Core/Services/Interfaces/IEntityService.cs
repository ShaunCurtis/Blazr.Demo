﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IEntityService<TEntity>
    where TEntity : class, IEntity 
{
    public string Url { get;}

    public string SingleTitle { get;}

    public string PluralTitle { get; }
}
