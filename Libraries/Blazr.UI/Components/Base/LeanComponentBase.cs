/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class LeanComponentBase : IComponent
{
    protected RenderFragment renderFragment;
    private RenderHandle _renderHandle;
    protected bool initialized;
    private bool _hasNeverRendered = true;
    private bool _hasPendingQueuedRender;
    private bool _hidden;

    [Parameter] public Boolean Hidden { get; set; } = false;

    public LeanComponentBase()
    {
        this.renderFragment = builder =>
        {
            if (!this.Hidden)
            {
                _hasPendingQueuedRender = false;
                _hasNeverRendered = false;
                this.BuildRenderTree(builder);
            }
        };
    }

    void IComponent.Attach(RenderHandle renderHandle)
        => _renderHandle = renderHandle;

    public virtual async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        var shouldRender = this.ShouldRenderOnParameterChange(initialized);

        if (_hasNeverRendered || shouldRender || _renderHandle.IsRenderingOnMetadataUpdate)
        {
            await this.OnParametersChangedAsync(!initialized);
            this.Render();
        }

        this.initialized = true;
    }

    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }

    protected virtual ValueTask OnParametersChangedAsync(bool firstRender)
        => ValueTask.CompletedTask;

    protected virtual bool ShouldRenderOnParameterChange(bool initialized)
    {
        var tripwire = new TripWire();

        tripwire.TripOnFalse(this.Hidden == _hidden);
        _hidden = this.Hidden;

        return tripwire.IsTripped;
    }

    private void Render()
    {
        if (_hasPendingQueuedRender)
            return;

        _hasPendingQueuedRender = true;

        try
        {
            _renderHandle.Render(renderFragment);
        }
        catch
        {
            _hasPendingQueuedRender = false;
            throw;
        }
    }

    protected void StateHasChanged()
        => _renderHandle.Dispatcher.InvokeAsync(Render);
}
