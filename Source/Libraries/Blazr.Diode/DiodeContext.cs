/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public class DiodeContext<TIdentity, TRecord>
    where TRecord : class, IDiodeRecord<TIdentity>, new()
{
    private TRecord _immutableItem;
    private int _stateChanges = 0;
    
    public TIdentity Id => _immutableItem.Id;
    public TRecord Item => _immutableItem;
    public int StateChanges => _stateChanges;
    public DiodeState State { get; private set; }

    /// <summary>
    /// Event raised when a context mutates
    /// </summary>
    public event EventHandler<DiodeEventArgs>? StateHasChanged;

    internal DiodeContext(TRecord item, DiodeState? state = null)
    {
        _immutableItem = item;
        this.State = state ?? DiodeState.Clean;
    }

    public DiodeResult Update(DiodeMutationDelegate<TIdentity, TRecord> mutation, object? sender = null)
    {
        var mutationResult = mutation.Invoke(this);

        if (mutationResult.Item == _immutableItem)
            return DiodeResult.Failure("No changes to apply.");

        _stateChanges++;
        _immutableItem = mutationResult.Item;

        if (this.State == DiodeState.Clean)
            this.State = DiodeState.Modified;

        this.NotifyStateHasChanged(sender);

        return DiodeResult.Success();
    }

    public DiodeResult Delete(object? sender = null)
    {
        this.State = DiodeState.Deleted;
        this.NotifyStateHasChanged(sender);
        return DiodeResult.Success();
    }

    public void Persisted(object? sender = null)
    {
        this.State = DiodeState.Clean;
        _stateChanges = 0;
        this.NotifyStateHasChanged(sender);
    }

    private void NotifyStateHasChanged(object? sender = null)
    {
        this.StateHasChanged?.Invoke(sender, new(_immutableItem, State));
    }

    public static DiodeContext<TIdentity, TRecord> CreateNew(TRecord item)
    {
        return new(item, DiodeState.New);
    }

    public static DiodeContext<TIdentity, TRecord> CreateClean(TRecord item)
    {
        return new(item, DiodeState.Clean);
    }
}