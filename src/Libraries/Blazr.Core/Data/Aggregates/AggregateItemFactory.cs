/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public static class AggregateItemFactory
{
    public static AggregateItem<TItem> AsExisting<TItem>(TItem item)
        where TItem : class, IEntity, IStateEntity, IAggregateItem, new()
        => new AggregateItem<TItem>() { BaseItem = item, Item = item };

    public static AggregateItem<TItem> AsNew<TItem>(TItem item)
        where TItem : class, IEntity, IStateEntity, IAggregateItem, new()
        => new AggregateItem<TItem>() { BaseItem = null, Item = item };
}
