/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract partial class BlazrEditorForm<TRecord, TEditRecord, TEntity>
    : BlazrOwningComponentBase<IEditService<TRecord, TEditRecord, TEntity>>, IDisposable
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
    where TEntity : class, IEntity
{
    private bool _isNew = true;
    protected EditContext editContext = default!;
    protected string FormTitle = "Record Editor";

    [Parameter] public Guid Id { get; set; }

    [Parameter] public EventCallback ExitAction { get; set; }

    [CascadingParameter] public IModalDialog? Modal { get; set; }

    [Inject] private INotificationService<TEntity> NotificationService { get; set; } = default!;

    [Inject] protected IBlazrNavigationManager NavManager { get; set; } = default!;

    [Inject] protected IEntityService<TEntity> EntityService { get; set; } = default!;

    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;

    [Inject] protected IServiceProvider SPAServiceProvider { get; set; } = default!;

    protected BlazrNavigationManager? blazrNavManager => NavManager is BlazrNavigationManager ? NavManager as BlazrNavigationManager : null;

    protected string? alertMessage;
    protected string alertColour = "alert-info";
    protected int alertTimeOut = 0;
    protected Guid alertId;
    protected bool isConfirmDelete = false;

    public ComponentState LoadState { get; protected set; } = ComponentState.New;

    protected bool IsModal => this.Modal != null;

    protected bool IsDirty => this.Service.EditModel.IsDirty;

    protected bool IsNew => this.Service.EditModel.IsNew;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        await PreLoadRecordAsync(_isNew);
        if (_isNew)
        {
            if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
                this.FormTitle = $"{this.EntityUIService.SingleTitle} Editor";

            this.Service.SetServices(this.SPAServiceProvider);

            this.LoadState = ComponentState.Loading;

            if (Id != Guid.Empty)
                await this.Service.LoadRecordAsync(Id);
            else
                await this.Service.GetNewRecordAsync(await GetNewRecord());

            this.editContext = new EditContext(this.Service.EditModel);
            this.editContext.OnFieldChanged += FieldChanged;

            if (this.blazrNavManager is not null)
            {
                this.blazrNavManager.BrowserExitAttempted += FailedExitAttempt;
                this.blazrNavManager.NavigationEventBlocked += FailedRoutingAttempt;
            }

            this.LoadState = ComponentState.Loaded;
        }

        await base.SetParametersAsync(ParameterView.Empty);
        _isNew = false;
    }

    public virtual Task PreLoadRecordAsync(bool isNew)
        => Task.CompletedTask;

    protected virtual Task<TRecord> GetNewRecord()
        => Task.FromResult(new TRecord());

    private void FieldChanged(object? sender, FieldChangedEventArgs e)
    {
        this.blazrNavManager?.SetLockState(this.IsDirty);
        this.isConfirmDelete = false;
        this.InvokeAsync(StateHasChanged);
    }

    private void FailedRoutingAttempt(object? sender, EventArgs e)
        => this.SetMessage("You can't navigate away from this form with unsaved data", "alert-warning");

    private void FailedExitAttempt(object? sender, EventArgs e)
    => this.SetMessage("You attempted to exit the application", "alert-warning");

    protected void ResetRecord()
    {
        this.Service.EditModel.Reset();
        this.SetMessage("Fields reset to database values", "alert-info");
    }

    protected async Task<bool> SaveRecord()
    {
        var result = false;
        if (this.editContext.Validate())
        {
            result = await this.Service.UpdateRecordAsync();
            this.blazrNavManager?.SetLockState(this.IsDirty);
            if (result)
                this.SetMessage("Record Saved", "alert-success");
            else
                this.SetMessage(this.Service.Message ?? "Problem saving record", "alert-danger");
        }
        else
            this.SetMessage("There are validation problems", "alert-danger");

        return result;
    }

    protected virtual async Task SaveRecordAndExit()
    {
        if (await this.SaveRecord())
            await DoExit();
    }

    protected virtual async Task<bool> AddRecord()
    {
        var hasSaved = false;
        if (this.editContext.Validate())
        {
            var result = await this.Service.AddRecordAsync();
            this.blazrNavManager?.SetLockState(this.IsDirty);
            if (result)
            {
                this.SetMessage("Record Added", "alert-success");
                await this.Service.LoadRecordAsync(this.Service.EditModel.Id);
                hasSaved = true;
            }
            else
                this.SetMessage(this.Service.Message ?? "Problem adding record", "alert-danger");
        }
        else
            this.SetMessage("There are validation problems", "alert-danger");

        return hasSaved;
    }

    protected virtual async Task AddRecordAndExit()
    {
        if (await this.AddRecord())
            await DoExit();
    }

    protected void ClearConfirmDelete()
        => this.isConfirmDelete = false;

    protected void ShowConfirmDelete()
        => this.isConfirmDelete = true;

    protected void OnRecordChanged(object? sender, EventArgs e)
        => this.InvokeAsync(StateHasChanged);

    protected void OnEditStateChanged(object? sender, EditStateEventArgs e)
    {
        this.blazrNavManager?.SetLockState(e.IsDirty);
        this.InvokeAsync(StateHasChanged);
    }

    protected async void Exit()
        => await DoExit();

    protected async void ExitWithoutSaving()
    {
        this.blazrNavManager?.SetLockState(false);
        await DoExit();
    }

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

    protected virtual void BaseExit()
        => this.NavManager?.NavigateTo($"/{this.EntityUIService.Url}");

    protected void SetMessage(string message, string colour)
    {
        this.alertMessage = message;
        this.alertColour = colour;
        this.alertTimeOut = 0;
        this.alertId = Guid.NewGuid();
        this.StateHasChanged();
    }

    public virtual void Dispose()
    {
        if (this.editContext is not null)
            this.editContext.OnFieldChanged -= FieldChanged;
        if (this.blazrNavManager is not null)
        {
            this.blazrNavManager.BrowserExitAttempted -= FailedExitAttempt;
            this.blazrNavManager.NavigationEventBlocked -= FailedRoutingAttempt;
        }
    }
}
