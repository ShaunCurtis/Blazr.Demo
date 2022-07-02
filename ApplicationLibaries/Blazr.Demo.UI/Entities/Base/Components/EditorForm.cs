/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.UI;

public abstract partial class EditorForm<TRecord, TEditRecord, TService>
    : OwningComponentBase, IDisposable
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
    where TService : class, IEntityService
{
    protected EditContext editContext = default!;

    [Parameter] public Guid Id { get; set; }

    [Parameter] public string? RecordUrl { get; set; }

    [Parameter] public EventCallback ExitAction { get; set; }

    [CascadingParameter] public IModalDialog? Modal { get; set; }

    [Inject] private ICrudService<TRecord, TEditRecord>? _crudService { get; set; }

    [Inject] private INotificationService<TService>? _notificationService { get; set; }

    [Inject] protected IBlazrNavigationManager? NavManager { get; set; }

    protected ICrudService<TRecord, TEditRecord> crudService => _crudService!;
    private INotificationService<TService> notificationService => _notificationService!;
    protected BlazrNavigationManager? blazrNavManager => NavManager is BlazrNavigationManager ? NavManager as BlazrNavigationManager : null;

    protected string? alertMessage;
    protected string alertColour = "alert-info";
    protected int alertTimeOut = 0;
    protected Guid alertId;
    protected bool isConfirmDelete = false;

    public ComponentState LoadState { get; protected set; } = ComponentState.New;

    protected bool IsModal => this.Modal != null;

    protected bool IsDirty => this.crudService.EditModel.IsDirty;

    protected bool IsNew => this.crudService.EditModel.IsNew;

    private string _recordUrl;

    protected string ListUrl => string.IsNullOrWhiteSpace(this.RecordUrl) ? _recordUrl : this.RecordUrl;

    public EditorForm()
    {
        var name = new TRecord().GetType().Name
            .Replace("Dbo", "")
            .Replace("Dvo", "");

        _recordUrl = name;
    }

    protected async override Task OnInitializedAsync()
    {

        this.LoadState = ComponentState.Loading;

        if (Id != Guid.Empty)
            await this.crudService.GetRecordAsync(Id);
        else 
            await this.crudService.GetNewRecordAsync(await GetNewRecord());

        this.editContext = new EditContext(this.crudService.EditModel);
        this.editContext.OnFieldChanged += FieldChanged;
        
        if (this.blazrNavManager is not null)
        {
            this.blazrNavManager.BrowserExitAttempted += FailedExitAttempt;
            this.blazrNavManager.NavigationEventBlocked += FailedRoutingAttempt;
        }
        
        this.LoadState = ComponentState.Loaded;
    }

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
        this.crudService.EditModel.Reset();
        this.SetMessage("Fields reset to database values", "alert-info");
    }

    protected async Task SaveRecord()
    {
        if (this.editContext.Validate())
        {
            var result = await this.crudService.UpdateRecordAsync();
            this.blazrNavManager?.SetLockState(this.IsDirty);
            if (result)
                this.SetMessage("Record Saved", "alert-success");
            else
                this.SetMessage("Problem saving record", "alert-danger");
        }
        else
            this.SetMessage("There are validation problems", "alert-danger");
    }

    protected virtual async Task<bool> AddRecord()
    {
        var hasSaved = false;
        if (this.editContext.Validate())
        {
            var result = await this.crudService.AddRecordAsync();
            this.blazrNavManager?.SetLockState(this.IsDirty);
            if (result)
            {
                this.SetMessage("Record Added", "alert-success");
                await this.crudService.GetRecordAsync(this.crudService.EditModel.Id);
                hasSaved = true;
            }
            else
                this.SetMessage("Problem adding record", "alert-danger");
        }
        else
            this.SetMessage("There are validation problems", "alert-danger");

        return hasSaved;
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
        => this.NavManager?.NavigateTo("/");

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
