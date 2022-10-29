/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

/// <summary>
/// Base minimum footprint component for building simple Components
/// No events
/// </summary>
public abstract class BlazrComponentBase : IComponent
{
    protected RenderFragment renderFragment;
    protected internal RenderHandle renderHandle;
    protected bool hasPendingQueuedRender = false;
    protected internal bool hasNeverRendered = true;

    /// <summary>
    /// New method
    /// caches a copy of the Render code
    /// Detects if the component shoud be rendered and if not doesn't render ant content
    /// </summary>
    public BlazrComponentBase()
    {
        this.renderFragment = builder =>
        {
            hasPendingQueuedRender = false;
            hasNeverRendered = false;
            this.BuildRenderTree(builder);
        };
    }

    /// <summary>
    /// Default Render method required by Razor to compile the Razor markup to.
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }

    /// <summary>
    /// Method to queue the component Render Fragment onto the Renderer's Render Queue
    /// Only adds it if there are no other copies already queued
    /// </summary>
    protected void StateHasChanged()
    {
        if (hasPendingQueuedRender)
            return;

        hasPendingQueuedRender = true;
        renderHandle.Render(this.renderFragment);
    }

    /// <summary>
    /// StateHasChanged Method that is invoked on the UI Thread
    /// Do not call through InvokeAsync, it already does it.
    /// </summary>
    protected void InvokeStateHasChanged()
        => renderHandle.Dispatcher.InvokeAsync(StateHasChanged);

    /// <summary>
    ///  IComponent implementation
    ///  Gets and saves the provided RenderHandle
    /// </summary>
    /// <param name="renderHandle"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Attach(RenderHandle renderHandle)
        => this.renderHandle = renderHandle;

    /// <summary>
    ///  IComponent implementation
    /// Called by the Renderer at initialization and whenever any of the requested Parameters change
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        this.StateHasChanged();
        return Task.CompletedTask;
    }
}
