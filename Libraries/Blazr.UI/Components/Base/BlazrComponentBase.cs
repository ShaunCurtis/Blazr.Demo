//  ===============================================================================
//  This is direct copy of ComponentBase with minor changes
//  These are changes to top class variables to protected
//      _initialized;
//      _hasNeverRendered
//      _hasPendingQueuedRender;
//      _hasCalledOnAfterRender;
//   ==============================================================================

/// ============================================================
/// Change Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrComponentBase : IComponent, IHandleEvent, IHandleAfterRender
{
    protected RenderFragment _renderFragment;
    private RenderHandle _renderHandle;
    protected bool _initialized;
    protected bool _hasNeverRendered = true;
    protected bool _hasPendingQueuedRender;
    protected bool _hasCalledOnAfterRender;

    public BlazrComponentBase()
    {
        _renderFragment = builder =>
        {
            _hasPendingQueuedRender = false;
            _hasNeverRendered = false;
            BuildRenderTree(builder);
        };
    }

    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }

    protected virtual void OnInitialized() { }

    protected virtual Task OnInitializedAsync() => Task.CompletedTask;

    protected virtual void OnParametersSet() { }

    protected virtual Task OnParametersSetAsync() => Task.CompletedTask;

    protected void StateHasChanged()
    {
        if (_hasPendingQueuedRender)
            return;

        if (_hasNeverRendered || ShouldRender() || _renderHandle.IsRenderingOnMetadataUpdate)
        {
            _hasPendingQueuedRender = true;

            try
            {
                _renderHandle.Render(_renderFragment);
            }
            catch
            {
                _hasPendingQueuedRender = false;
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

    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (!_initialized)
        {
            _initialized = true;

            return this.RunInitAndSetParametersAsync();
        }
        else
            return this.CallOnParametersSetAsync();
    }

    private async Task RunInitAndSetParametersAsync()
    {
        this.OnInitialized();

        var task = this.OnInitializedAsync();

        if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
        {
            this.StateHasChanged();

            try
            {
                await task;
            }
            catch // avoiding exception filters for AOT runtime support
            {
                if (!task.IsCanceled)
                    throw;
            }
        }

        await this.CallOnParametersSetAsync();
    }

    private Task CallOnParametersSetAsync()
    {
        this.OnParametersSet();

        var task = this.OnParametersSetAsync();
        var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
            task.Status != TaskStatus.Canceled;

        StateHasChanged();

        return shouldAwaitTask ?
            this.CallStateHasChangedOnAsyncCompletion(task) :
            Task.CompletedTask;
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
        StateHasChanged();
    }

    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        var task = callback.InvokeAsync(arg);
        var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled;

        StateHasChanged();

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