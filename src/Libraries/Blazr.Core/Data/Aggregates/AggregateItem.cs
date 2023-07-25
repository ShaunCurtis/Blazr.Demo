/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class AggregateItem<TItem>
    where TItem : class, IEntity, IStateEntity, IAggregateItem, new()
{
    private CommandResult FailOnUidCheck = CommandResult.Failure("Can't update - the Uid of a submitted Item doesn't match the UI of the stored item.");

    public TItem? BaseItem { get; internal set; }
    public TItem Item { get; internal set; } = new();
    public int BaseStateCode => this.BaseItem?.StateCode ?? AppStateCodes.New; 
    public EntityUid Uid => this.Item.Uid;
    public bool IsDirty => this.Item != this.BaseItem;

    internal AggregateItem() { }

    public CommandResult Update(TItem item)
    {
        if (item.Uid != this.Item.Uid)
            return this.FailOnUidCheck;
    
        this.Item = item;
        return CommandResult.Success();
    }

    public CommandResult Update(TItem actualItem, TItem baseStateSetItem)
    {
        if (actualItem.Uid != this.Item.Uid)
            return this.FailOnUidCheck;

        var sameAsBase = BaseItem == baseStateSetItem;

        this.Item = sameAsBase
            ? baseStateSetItem
            : actualItem;

        return CommandResult.Success();
    }

    public void Reset()
    {
        if (BaseItem is not null)
            this.Item = this.BaseItem;
    }

    public void SetAsSaved()
        => this.BaseItem = this.Item;

    public void SetAsNew(TItem newItem)
    {
        this.BaseItem = null;
        this.Item = this.Item;
    }
}
