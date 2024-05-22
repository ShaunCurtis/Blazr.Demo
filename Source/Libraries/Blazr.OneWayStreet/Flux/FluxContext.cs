/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Flux;

public class FluxContext<TIdentity, TRecord>
    where TRecord : class, IFluxRecord<TIdentity>, new()
{
    private TRecord _immutableItem;
    private int _stateChanges = 0;
    
    public TIdentity Id => _immutableItem.Id;
    public TRecord Item => _immutableItem;
    public int StateChanges => _stateChanges;
    public FluxState State { get; private set; }

    /// <summary>
    /// Event raised when a context mutates
    /// </summary>
    public event EventHandler<FluxEventArgs>? StateHasChanged;

    internal FluxContext(TRecord item, FluxState? state = null)
    {
        _immutableItem = item;
        this.State = state ?? FluxState.Clean;
    }

    public IDataResult Update(FluxMutationDelegate<TIdentity, TRecord> mutation, object? sender = null)
    {
        var mutationResult = mutation.Invoke(this);

        if (mutationResult.Item == _immutableItem)
            return DataResult.Failure("No changes to apply.");

        _stateChanges++;
        _immutableItem = mutationResult.Item;

        if (this.State == FluxState.Clean)
            this.State = FluxState.Modified;

        this.NotifyStateHasChanged(sender);

        return DataResult.Success();
    }

    public IDataResult Delete(object? sender = null)
    {
        this.State = FluxState.Deleted;
        this.NotifyStateHasChanged(sender);
        return DataResult.Success();
    }

    public void Persisted(object? sender = null)
    {
        this.State = FluxState.Clean;
        _stateChanges = 0;
        this.NotifyStateHasChanged(sender);
    }

    private void NotifyStateHasChanged(object? sender = null)
    {
        this.StateHasChanged?.Invoke(sender, new(_immutableItem, State));
    }

    public static FluxContext<TIdentity, TRecord> CreateNew(TRecord item)
    {
        return new(item, FluxState.New);
    }

    public static FluxContext<TIdentity, TRecord> CreateClean(TRecord item)
    {
        return new(item, FluxState.Clean);
    }
}