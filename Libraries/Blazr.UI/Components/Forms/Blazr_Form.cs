/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class Blazr_Form<TService, TEntity>
    :    UITemplatedComponentBase, IHandleEvent, IHandleAfterRender
    where TService : class
    where TEntity : class, IEntity
{
    protected bool isNew = true;
    protected string FormTitle = "Record Viewer";

    /// <summary>
    /// Id for the record
    /// </summary>
    [Parameter] public Guid Id { get; set; }

    /// <summary>
    /// Pick up a Cascaded IModalDialog if we're hosted within a Modal Dialog context
    /// </summary>
    [CascadingParameter] public IModalDialog? Modal { get; set; }

    /// <summary>
    /// Specify a specific exit mechanism
    /// </summary>
    [Parameter] public EventCallback ExitAction { get; set; }

    // Get all the DI Services we need
    [Inject] protected NavigationManager NavManager { get; set; } = default!;

    [Inject] protected ModalService ModalService { get; set; } = default!;

    [Inject] protected INotificationService<TEntity> NotificationService { get; set; } = default!;

    [Inject] protected IEntityService<TEntity> EntityService { get; set; } = default!;

    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;

    [Inject] protected IServiceProvider SPAServiceProvider { get; set; } = default!;

    /// <summary>
    /// The component state
    /// </summary>
    public ComponentState LoadState { get; protected set; } = ComponentState.New;

    /// <summary>
    /// Method in the component event chain prior to loading the record 
    /// </summary>
    /// <returns></returns>
    protected virtual Task PreLoadRecordAsync()
        => Task.CompletedTask;

    /// <summary>
    /// Method to load the Record
    /// </summary>
    /// <param name="render"></param>
    /// <returns></returns>
    private Task LoadRecordAsync(bool render = false)
        => Task.CompletedTask;

    /// <summary>
    /// Default Exit for buttons
    /// </summary>
    protected virtual async void Exit()
        => await DoExit();

    /// <summary>
    /// Exit method that figures out the correct exit method i.e. exit from a modal dialog or exit from a form
    /// </summary>
    /// <returns></returns>
    protected async Task DoExit()
    {
        // If we're in a modal context, call Close on the cascaded Modal object
        if (this.Modal is not null)
        {
            this.Modal.Close(ModalResult.OK());
            return;
        }

        // If there's a delegate registered on the ExitAction, execute it. 
        if (ExitAction.HasDelegate)
        {
            await ExitAction.InvokeAsync();
            return;
        }

        // fallback action is to navigate to root
        this.BaseExit();
    }

    /// <summary>
    /// Exit to the Entity defined Url
    /// </summary>
    protected void BaseExit()
        => this.NavManager?.NavigateTo($"/{this.EntityUIService.Url}");

    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        await callback.InvokeAsync(arg);
        this.StateHasChanged();
    }

    Task IHandleAfterRender.OnAfterRenderAsync()
        => Task.CompletedTask;
}
