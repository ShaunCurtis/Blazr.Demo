/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.UI;

public class ViewerForm<TRecord, TService> 
    : OwningComponentBase
    where TRecord : class, new()
    where TService : class, IEntityService
{
    protected string RecordUrl;
    protected Type? EditControl;
    protected NavigationManager NavManager => _navManager!;
    protected IListService<TRecord> ListService => _listService!;
    protected ModalService modalService => _modalService!;
    protected INotificationService<TService> notificationService => _notificationService!;

    [Parameter] public Guid Id { get; set; }

    [Parameter] public EventCallback ExitAction { get; set; }

    [CascadingParameter] public IModalDialog? Modal { get; set; }

    [Inject] private NavigationManager? _navManager { get; set; }

    [Inject] private ModalService? _modalService { get; set; }

    [Inject] private IListService<TRecord>? _listService { get; set; }

    [Inject] private INotificationService<TService>? _notificationService { get; set; }

    public ComponentState LoadState { get; protected set; } = ComponentState.New;

    public ViewerForm()
    {
        var name = new TRecord().GetType().Name
            .Replace("Dbo", "")
            .Replace("Dvo", "");

        this.RecordUrl = name;
    }

    protected async override Task OnInitializedAsync()
    {
        await this.LoadRecord();
        this.notificationService.RecordChanged += OnChange;
    }

    private async Task LoadRecord(bool render = false)
    {
        this.LoadState = ComponentState.Loading;
        await this.ListService.GetRecordAsync(Id);
        this.LoadState = ComponentState.Loaded;
        if (render)
            await this.InvokeAsync(this.StateHasChanged);
    }

    private async void OnChange(object? sender, RecordEventArgs e)
    { 
        if (this.IsThisRecord(e.RecordId))
            await LoadRecord(true);
    }

    protected virtual bool IsThisRecord(Guid Id)
        => true;

    protected virtual async Task EditRecordAsync()
    {
        if (this.Modal is null && this.modalService.IsModalFree && this.EditControl is not null)
        {
            var options = new ModalOptions();
            options.ControlParameters.Add("Id", this.Id);
            options = this.GetEditOptions(options);
            await this.modalService.Modal.ShowAsync(this.EditControl, options);
        }
        else
            this.NavManager!.NavigateTo($"/{this.RecordUrl}/edit/{Id}");
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
        => this.NavManager?.NavigateTo("/");
}
