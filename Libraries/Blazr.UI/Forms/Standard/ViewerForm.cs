/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ViewerForm<TRecord, TEntity>
    : OwningComponentBase<IReadService<TRecord, TEntity>>, IDisposable
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    private bool _isNew = true;
    protected Type? EditControl;

    [Parameter] public Guid Id { get; set; }

    [Parameter] public EventCallback ExitAction { get; set; }

    [CascadingParameter] public IModalDialog? Modal { get; set; }

    [Inject] protected NavigationManager NavManager { get; set; } = default!;

    [Inject] protected ModalService ModalService { get; set; } = default!;

    [Inject] protected INotificationService<TEntity> NotificationService { get; set; } = default!;

    [Inject] protected IEntityService<TEntity> EntityService { get; set; } = default!;

    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;

    public ComponentState LoadState { get; protected set; } = ComponentState.New;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (_isNew)
        {
            await PreLoadRecordAsync();
            this.Service.SetNotificationService(this.NotificationService);
            await this.LoadRecordAsync();
            this.NotificationService.RecordChanged += OnChange;
        }

        await base.SetParametersAsync(ParameterView.Empty);
        _isNew = false;
    }

    protected virtual Task PreLoadRecordAsync()
        => Task.CompletedTask;

    private async Task LoadRecordAsync(bool render = false)
    {
        this.LoadState = ComponentState.Loading;
        await this.Service.GetRecordAsync(Id);
        this.LoadState = ComponentState.Loaded;
        if (render)
            await this.InvokeAsync(this.StateHasChanged);
    }

    private async void OnChange(object? sender, RecordEventArgs e)
    {
        if (this.IsThisRecord(e.RecordId))
            await LoadRecordAsync(true);
    }

    protected virtual bool IsThisRecord(Guid Id)
    {
        if (this.Service.Record is IRecord)
            return ((IRecord)this.Service.Record).Id == Id;

        return true;
    }

    protected virtual async Task EditRecordAsync()
    {
        if (this.Modal is null && this.ModalService.IsModalFree && this.EditControl is not null)
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
        else
            this.NavManager!.NavigateTo($"/{this.EntityUIService.Url}/edit/{Id}");
    }

    protected virtual ModalOptions GetEditOptions(ModalOptions? options)
        => options ?? new ModalOptions();

    protected async void Exit()
        => await DoExit();

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

    public void Dispose()
        => this.NotificationService.RecordChanged -= OnChange;

}
