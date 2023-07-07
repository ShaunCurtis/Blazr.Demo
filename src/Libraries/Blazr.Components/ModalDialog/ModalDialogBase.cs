/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components;

public abstract class ModalDialogBase : BlazrControlBase, IModalDialog
{
    public ModalOptions Options { get; protected set; } = new ModalOptions();

    public bool Display { get; protected set; }

    public bool IsActive => this.ModalContentType is not null;

    protected TaskCompletionSource<ModalResult> _ModalTask { get; set; } = new TaskCompletionSource<ModalResult>();

    protected Type? ModalContentType = null;

    public Task<ModalResult> ShowAsync<TModal>(ModalOptions options) where TModal : IComponent
    {
        this.ModalContentType = typeof(TModal);
        this.Options = options ??= this.Options;
        this._ModalTask = new TaskCompletionSource<ModalResult>();
        this.Display = true;
        InvokeAsync(StateHasChanged);
        return this._ModalTask.Task;
    }

    public Task<ModalResult> ShowAsync(Type control, ModalOptions options)
    {
        if (!(typeof(IComponent).IsAssignableFrom(control)))
            throw new InvalidOperationException("Passed control must implement IComponent");

        this.Options = options ??= this.Options;
        this._ModalTask = new TaskCompletionSource<ModalResult>();
        this.ModalContentType = control;
        this.Display = true;
        InvokeAsync(StateHasChanged);
        return this._ModalTask.Task;
    }

    public async Task<bool> SwitchAsync<TModal>(ModalOptions options) where TModal : IComponent
    {
        this.ModalContentType = typeof(TModal);
        this.Options = options ??= this.Options;
        await InvokeAsync(StateHasChanged);
        return true;
    }

    public async Task<bool> SwitchAsync(Type control, ModalOptions options)
    {
        if (!(typeof(IComponent).IsAssignableFrom(control)))
            throw new InvalidOperationException("Passed control must implement IComponent");

        this.ModalContentType = control;
        this.Options = options ??= this.Options;
        await InvokeAsync(StateHasChanged);
        return true;
    }

    /// <summary>
    /// Method to update the state of the display based on UIOptions
    /// </summary>
    /// <param name="options"></param>
    public void Update(ModalOptions? options = null)
    {
        this.Options = options ??= this.Options;
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Method called by the dismiss button to close the dialog
    /// sets the task to complete, show to false and renders the component (which hides it as show is false!)
    /// </summary>
    public async void Dismiss()
    {
        _ = this._ModalTask.TrySetResult(ModalResult.Cancel());
        await Reset();
    }

    /// <summary>
    /// Method called by child components through the cascade value of this component
    /// sets the task to complete, show to false and renders the component (which hides it as show is false!)
    /// </summary>
    /// <param name="result"></param>
    public async void Close(ModalResult result)
    {
        _ = this._ModalTask.TrySetResult(result);
        await Reset();
    }

    private async Task Reset()
    {
        this.Display = false;
        this.ModalContentType = null;
        await InvokeAsync(StateHasChanged);
    }
}

