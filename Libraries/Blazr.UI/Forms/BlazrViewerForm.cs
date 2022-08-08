/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrViewerForm<TRecord, TEntity>
    : BlazrOwningComponentBase<IReadService<TRecord, TEntity>>, IDisposable, IHandleEvent, IHandleAfterRender
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    private bool _isNew = true;
    protected virtual Type? EditControl => this.EntityUIService.EditForm;
    protected string FormTitle = "Record Viewer";

    /// <summary>
    /// Id for the record
    /// </summary>
    [Parameter] public Guid Id { get; set; }

    /// <summary>
    /// Specify a specific exit mechanism
    /// </summary>
    [Parameter] public EventCallback ExitAction { get; set; }

    /// <summary>
    /// Pick up a Cascaded IModalDialog if one is configured on the parent
    /// </summary>
    [CascadingParameter] public IModalDialog? Modal { get; set; }

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
    /// Overloaded component SetParametersAsync
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        // Applies the component parameter changes
        parameters.SetParameterProperties(this);

        if (_isNew)
        {
            // Get the Title for the form
            if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
                this.FormTitle = $"{this.EntityUIService.SingleTitle} Viewer";

            // We're using Owning so creating our own services container for our Read Service
            // This service needs access to the SPA's Scoped services.  So calling this method on the service
            // sets those services to the correct SPA level instances of those objects
            this.Service.SetServices(SPAServiceProvider);

            // Loads the record
            await PreLoadRecordAsync();
            await this.LoadRecordAsync();

            // Sets up the evnt handler for record changes
            this.NotificationService.RecordChanged += OnChange;
        }

        // Calls the base version.
        // This does all the usual ComponentBase OnInitialized{Async} and OnParametersSet{Async} stuff
        await base.SetParametersAsync(ParameterView.Empty);

        // We aren't new any more
        _isNew = false;
    }

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
    private async Task LoadRecordAsync(bool render = false)
    {
        this.LoadState = ComponentState.Loading;

        if (await this.Service.GetRecordAsync(Id))
            this.LoadState = ComponentState.Loaded;
        else
            this.LoadState = ComponentState.UnAuthorized;

        if (render)
            await this.InvokeAsync(this.StateHasChanged);
    }

    /// <summary>
    /// Event handler for a record changed notificatiion
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnChange(object? sender, RecordEventArgs e)
    {
        if (this.IsThisRecord(e.RecordId))
            await LoadRecordAsync(true);
    }

    /// <summary>
    /// Checks if we are requested the existing loaded record
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    protected virtual bool IsThisRecord(Guid Id)
    {
        if (this.Service.Record is IRecord)
            return ((IRecord)this.Service.Record).Uid == Id;

        return true;
    }

    /// <summary>
    /// Method to switch to the Editor for the record
    /// </summary>
    /// <returns></returns>
    protected virtual async Task EditRecordAsync()
    {
        // There are three possible options:
        //     1. We aren't in a modal but there's a modal available and we have an editform defined
        //     2. We are in a modal and want to switch the control in the modal to the edit form
        //     3. We don't have all the options available to use a modal so navigate to the edit form 
        //         (We don't know if one exists, but that's not our problem!)
        if (this.Modal is not null && this.ModalService.IsModalFree && this.EditControl is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", this.Id);
            options = this.GetEditOptions(options);
            await this.ModalService.Modal.ShowAsync(this.EditControl, options);
        }
        else if (this.Modal is not null && this.EditControl is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", this.Id);
            options = this.GetEditOptions(options);
            await this.ModalService.Modal.SwitchAsync(this.EditControl, options);
        }
        // this needs testing in normal form for new component base
        else
            this.NavManager!.NavigateTo($"/{this.EntityUIService.Url}/edit/{Id}");
    }

    /// <summary>
    /// Method to let us add some extra options to the ModalOptions before we open the modal dialog
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ModalOptions GetEditOptions(ModalOptions? options)
        => options ?? new ModalOptions();

    /// <summary>
    /// Default Exit for buttons
    /// </summary>
    protected async void Exit()
        => await DoExit();

    /// <summary>
    /// Exit method that figures out the correct exit method i.e. exit from a modal dialog or exit from a form
    /// </summary>
    /// <returns></returns>
    protected virtual async Task DoExit()
    {
        // If we're in a modal context, call Close on the cascaded Modal object
        if (this.Modal is not null)
            this.Modal.Close(ModalResult.OK());

        // If there's a delegate registered on the ExitAction, execute it. 
        else if (ExitAction.HasDelegate)
            await ExitAction.InvokeAsync();

        // else fallback action is to navigate to root
        else
            this.BaseExit();
    }

    /// <summary>
    /// Exit to the Entity defined Url
    /// </summary>
    protected virtual void BaseExit()
        => this.NavManager?.NavigateTo($"/{this.EntityUIService.Url}");

    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        await callback.InvokeAsync(arg);
        Render();
    }

    Task IHandleAfterRender.OnAfterRenderAsync()
        => Task.CompletedTask;

    public void Dispose()
        => this.NotificationService.RecordChanged -= OnChange;

}
