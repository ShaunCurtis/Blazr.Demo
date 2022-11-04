/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class Blazr_Form<TEntity>
    :    UITemplatedComponentBase
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
    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;
    [Inject] protected IServiceProvider serviceProvider { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected ModalService ModalService { get; set; } = default!;

    /// <summary>
    /// The component state
    /// </summary>
    public ComponentState LoadState { get; protected set; } = ComponentState.New;

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
}
