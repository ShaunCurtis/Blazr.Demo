/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

// This is a base aggregate object representing a root item and a collection of associated items
public abstract class AggregateBase<TRootItem, TCollectionItem> : IIdentity, IStateEntity
    where TRootItem : class, IIdentity, IStateEntity, IAggregateItem, new()
    where TCollectionItem : class, IIdentity, IStateEntity, IAggregateItem, new()
{
    // both these items are set to readonly and private.  We don't expose their functionality to the outside world
    private readonly AggregateItemList<TCollectionItem> _items = new(Enumerable.Empty<TCollectionItem>());
    private readonly AggregateItem<TRootItem> _root;

    // Guid of the root entity
    public EntityUid Uid => _root.Uid;

    // The Entity State from the root
    public EntityState EntityState => this.Root.EntityState with { IsMutated = this.IsDirty };

    // We're dirty if either the root or the collection is dirty
    public bool IsDirty => _root.IsDirty | _items.IsDirty;

    public bool IsRootDirty => _root.IsDirty;

    // public expose the readonly record of the root
    public TRootItem Root => _root.Item;

    // public expose a readonly IEnumerable of the Item records.
    // You can't update the internal list or any items within it 
    public IEnumerable<TCollectionItem> LiveItems
        => _items.LiveItems.AsEnumerable();

    public IEnumerable<TCollectionItem> AllItems
        => _items.AllItems.AsEnumerable();

    // The no args creator creates an aggreagate from a new invoice
    public AggregateBase()
        => _root = AggregateItemFactory.AsNew(new TRootItem());

    // Creator to create an aggregate based on a root and collection
    public AggregateBase(TRootItem root, IEnumerable<TCollectionItem>? items)
    {
        _root = AggregateItemFactory.AsExisting(root);
        _items.Load(items ?? Enumerable.Empty<TCollectionItem>());
    }

    // Method to update the aggregate root item
    public CommandResult UpdateRoot(TRootItem root)
       => _root.Update(this.MutateRootItemState(root, _root.BaseStateCode));

    // Method to Delete the aggregate root item
    // Note this just marks the invoice for deletion, it doesn't remove it
    public CommandResult DeleteRoot(TRootItem root)
            => _root.Update(this.MutateRootItemState(root, AppStateCodes.Delete));

    // Method to produce a new Collection Item
    // Override this method is you need to populate it with any root data
    public virtual TCollectionItem NewCollectionItem()
        => new TCollectionItem();

    // Method to get a collection item
    public ItemQueryResult<TCollectionItem> GetCollectionItem(ItemQueryRequest request)
    {
        var item = _items.GetItem(request.Uid) ?? null;
        if (item == null)
            return ItemQueryResult<TCollectionItem>.Failure("No Item Exists");

        return ItemQueryResult<TCollectionItem>.Success(item);
    }

    // Method to delete a collection item
    // Note that it sets the state to deleted, it doesn't actually remove the item from the collection
    public CommandResult RemoveCollectionItem(TCollectionItem item)
    {
        var result = this._items.SaveItem(this.MutateCollectionItemState(item, AppStateCodes.Delete));

        if (result != null && result.Successful)
            this.NotifyUpdated();

        return result ?? CommandResult.Failure("No result returned.");
    }

    // Method to save a collection item.
    // It will update an existing item or add a new item if it csn't find and existing item
    public CommandResult SaveCollectionItem(TCollectionItem item)
    {
        var result = this._items.SaveItem(item);

        if (result != null && result.Successful)
            this.NotifyUpdated();

        return result ?? CommandResult.Failure("No result returned.");
    }

    // Method to check if a collection item is dirty
    public bool IsCollectionItemDirty(TCollectionItem item)
        => _items.ItemIsDirty(item);

    // Method to reset the items to the base set.
    // This will remove any items since the aggregate was last saved
    public void ResetCollectionItems()
    {
        _items.ResetItems();
        this.NotifyUpdated();
    }

    // Method to sets the aggregate to the current values
    // This is called on a successful persistence of the data to the data store
    // After this operation the root and collection items will all be clean
    public void SetAggregateAsSaved()
    {
        _root.SetAsSaved();
        _items.SetAsSaved();
    }

    // Method to reset the root and collection items to the base set.
    // After this operation the root and collection items will all be clean
    public void ResetAggregate()
    {
        _root.Reset();
        _items.ResetItems();
        this.NotifyUpdated();
    }

    // Method to set the root as new and clear the collection items.
    public void SetAggregateToNew()
    {
        _root.SetAsNew(this.MutateRootItemState(_root.Item, AppStateCodes.New));
        _items.Clear();
        this.NotifyUpdated();
    }

    // Method called when items change
    // Should be used to update any data within the aggregate that may change when an item within the aggregate changes
    protected virtual void NotifyUpdated() { }

    // Sets the state on a Root Item and returns a new copy of the item
    protected abstract TRootItem MutateRootItemState(TRootItem item, int state);

    // Sets the state on a Collection Item and returns a new copy of the item
    protected abstract TCollectionItem MutateCollectionItemState(TCollectionItem item, int state);

    // Method to check if an invoice item exists
    private bool LogItemExists(Guid uid)
        => _items.ItemExists(uid);
}
