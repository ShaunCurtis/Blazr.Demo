///  ===============================================================================
///
///  This is direct copy of ComponentBase with minor changes
///  1. These are changes to top class variables to protected
///      _initialized;
///      _hasNeverRendered
///      _hasPendingQueuedRender;
///   2. Changes to StateHasChanged.  
///         Adding a Render() method that replicates StateHasChanged
///         Changing StateHasChanged so it's always invoked on the UI Context thread
///   3. Changes to _renderFragment assignment in New
///         Adding a new Protected virtual RenderFragment ComponentRenderFrsgment 
///         that can overridden to change the rendering behaviour of the Component
///
///   ==============================================================================

/// ============================================================
/// Change Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrComponentBase : IComponent, IHandleEvent, IHandleAfterRender
{
    protected RenderFragment componentRenderFragment;
    private RenderHandle _renderHandle;
    protected bool initialized;
    protected bool hasNeverRendered = true;
    protected bool hasPendingQueuedRender;
    private bool _hasCalledOnAfterRender;

    public BlazrComponentBase()
    {
        componentRenderFragment = builder =>
        {
            hasPendingQueuedRender = false;
            hasNeverRendered = false;
            BuildComponent(builder);
        };
    }

    protected virtual void BuildComponent(RenderTreeBuilder builder)
    {
        if (this.TemplatedContent is not null)
        {
            this.TemplatedContent.Invoke(builder);
            return;
        }

        BuildRenderTree(builder);
    }

    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }

    protected virtual RenderFragment? TemplatedContent { get; }

    protected RenderFragment? Content => (builder) => this.BuildRenderTree(builder);

    protected virtual ValueTask OnParametersChangedAsync(bool firstRender)
        => ValueTask.CompletedTask;

    protected void StateHasChanged()
        => _renderHandle.Dispatcher.InvokeAsync(Render);

    internal protected void Render()
    {
        if (hasPendingQueuedRender)
            return;

        if (hasNeverRendered || ShouldRender() || _renderHandle.IsRenderingOnMetadataUpdate)
        {
            hasPendingQueuedRender = true;

            try
            {
                _renderHandle.Render(componentRenderFragment);
            }
            catch
            {
                hasPendingQueuedRender = false;
                throw;
            }
        }
    }

    protected virtual bool ShouldRender() => true;

    protected virtual void OnAfterRender(bool firstRender) { }

    protected virtual Task OnAfterRenderAsync(bool firstRender) => Task.CompletedTask;

    protected Task InvokeAsync(Action workItem)
        => _renderHandle.Dispatcher.InvokeAsync(workItem);

    protected Task InvokeAsync(Func<Task> workItem)
        => _renderHandle.Dispatcher.InvokeAsync(workItem);

    void IComponent.Attach(RenderHandle renderHandle)
    {
        if (_renderHandle.IsInitialized)
            throw new InvalidOperationException($"The render handle is already set. Cannot initialize a {nameof(ComponentBase)} more than once.");

        _renderHandle = renderHandle;
    }

    public virtual async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        await this.OnParametersChangedAsync(!initialized);

        Render();
    }

     private async Task CallStateHasChangedOnAsyncCompletion(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            if (task.IsCanceled)
                return;

            throw;
        }
        Render();
    }

    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        var task = callback.InvokeAsync(arg);
        var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled;

        Render();

        return shouldAwaitTask ?
            this.CallStateHasChangedOnAsyncCompletion(task) :
            Task.CompletedTask;
    }

    Task IHandleAfterRender.OnAfterRenderAsync()
    {
        var firstRender = !_hasCalledOnAfterRender;
        _hasCalledOnAfterRender |= true;

        OnAfterRender(firstRender);

        return this.OnAfterRenderAsync(firstRender);
    }
}