/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed class AggregateItemList<TItem>
    where TItem : class, IEntity, IStateEntity, IAggregateItem, new()
{
    private Guid Uid = Guid.NewGuid();
    private readonly List<AggregateItem<TItem>> _items = new();

    public bool IsDirty => _items.Any(item => item.IsDirty);

    public IEnumerable<TItem> LiveItems
        => _items
            .Where(item => !item.Item.EntityState.MarkedForDeletion)
            .Select(item => item.Item)
            .AsEnumerable();

    public IEnumerable<TItem> AllItems
        => _items
            .Select(item => item.Item)
            .AsEnumerable();

    public AggregateItemList(IEnumerable<TItem> items)
    {
        foreach (var item in items)
            _items.Add(AggregateItemFactory.AsExisting(item));
    }
    
    public TItem? GetItem(EntityUid uid)
        => _items.FirstOrDefault(item => item.Uid == uid)?.Item ?? null;

    public bool ItemExists(EntityUid uid)
        => _items.Any(item => item.Uid == uid);

    public bool ItemIsDirty(TItem collectionItem)
        => _items.FirstOrDefault(item => item.Uid == collectionItem.Uid)?.IsDirty ?? false;

    public CommandResult SaveItem(TItem updateItem)
    {
        var selectedItem = _items.FirstOrDefault(item => item.Uid == updateItem.Uid);
        if (selectedItem != null)
            selectedItem.Update(updateItem);

        else
            _items.Add(AggregateItemFactory.AsNew(updateItem));

        return CommandResult.Success();
    }

    public CommandResult DeleteItem(TItem item)
    {
        var selectedItem = _items.FirstOrDefault(item => item.Uid == item.Uid);
        if (selectedItem != null)
            selectedItem.Delete(item);

        else
            _items.Add(AggregateItemFactory.AsNew(item));

        return CommandResult.Success();
    }

    public CommandResult AddExistingItem(TItem newItem)
    {
        if (_items.Any(item => item.Uid == newItem.Uid))
            return CommandResult.Failure("An item with the Uid already exists");

        _items.Add(AggregateItemFactory.AsExisting(newItem));

        return CommandResult.Success();
    }

    public void Load(IEnumerable<TItem> items)
    {
        _items.Clear();
        if (items is null)
            return;

        foreach (var item in items)
            this.AddExistingItem(item);
    }

    public void ResetItem(TItem updateItem)
        =>_items.FirstOrDefault(item => item.Uid == updateItem.Uid)?.Reset();

    public void ResetItems()
    {
        var _deleteItems = _items.Where(item => item.BaseItem is null).ToList();
        _deleteItems.ForEach(item => _items.Remove(item));
        _items.ForEach(item => item.Reset());
    }

    public void SetAsSaved()
        => _items.ForEach(item => item.SetAsSaved());

    public void Clear()
        => _items.Clear();
}
