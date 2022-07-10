/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class FormBase : IComponent, IHandleEvent, IHandleAfterRender
{
    private readonly RenderFragment _renderFragment;
    private RenderHandle _renderHandle;
    private bool _initialized;
    private bool _hasNeverRendered = true;
    private bool _hasPendingQueuedRender;
    private bool _hasCalledOnAfterRender;

    public FormBase()
    {
        ContentMarkup = builder =>
        {
            this.BuildRenderTree(builder);
        };

        FormMarkup = ContentMarkup;

        _renderFragment = builder =>
        {
            _hasPendingQueuedRender = false;
            _hasNeverRendered = false;
            this.BuildComponent(builder);
        };
    }

    private void BuildComponent(RenderTreeBuilder builder)
    {
        builder.AddContent(0, this.FormMarkup);
    }

    protected virtual RenderFragment FormMarkup { get; set; }

    protected readonly RenderFragment ContentMarkup;

    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }

    protected virtual Task OnInitializedAsync() => Task.CompletedTask;

    protected virtual Task OnParametersSetAsync() => Task.CompletedTask;

    protected virtual Task FormLoadAsync() => Task.CompletedTask;

    protected virtual Task FormRefreshAsync() => Task.CompletedTask;

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

    public virtual async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (!_initialized)
        {
            await this.FormLoadAsync();

            _initialized = true;

            await this.RunInitAndSetParametersAsync();
        }
        else
        {
            await this.FormRefreshAsync();
            await this.CallOnParametersSetAsync();
        }
    }

    private async Task RunInitAndSetParametersAsync()
    {
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
        catch // avoiding exception filters for AOT runtime support
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
        var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
            task.Status != TaskStatus.Canceled;

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