/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.Logging;

namespace Blazr.UI;

public partial class PagedListFormBase<TRecord, TEntityService> : BlazrControlBase, IAsyncDisposable
    where TRecord : class, new()
    where TEntityService : class, IEntityService
{
    [Inject] protected IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] protected IUIEntityService<TEntityService> UIEntityService { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected ILogger<PagedListFormBase<TRecord, TEntityService>> Logger { get; set; } = default!;
    [Inject] protected INotificationService<TEntityService> NotificationService { get; set; } = default!;

    [Parameter] public string? FormTitle { get; set;}

    [Parameter] public Guid StateId { get; set; } = Guid.Empty;

    public IListPresenter<TRecord, TEntityService> Presenter { get; set; } = default!;

    protected IModalDialog? modalDialog;
    protected string formTitle => this.FormTitle ?? $"List of {this.UIEntityService.PluralDisplayName}";

    private IDisposable? _disposable;

    protected override Task OnParametersSetAsync()
    {
        if (this.NotInitialized)
        {
            // Gets an instance of the Presenter from the Service Provider
            this.Presenter = ServiceProvider.GetComponentService<IListPresenter<TRecord, TEntityService>>() ?? default!;

            // Ensure we have  Presenter
            ArgumentNullException.ThrowIfNull(nameof(this.Presenter));

            // assign the Presenter if it impleements IDisposable
            _disposable = this.Presenter as IDisposable;
            Presenter.StateId = this.StateId;
        }

        return Task.CompletedTask;
    }

    protected virtual async Task OnEditAsync(TRecord record)
    {
        var id = RecordUtilities.GetIdentity(record);
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        if (modalDialog is not null && this.UIEntityService.EditForm is not null)
        {
            await modalDialog.ShowAsync(this.UIEntityService.EditForm, options);
            this.StateHasChanged();
        }
        else
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/edit/{id}");
    }

    protected virtual async Task OnViewAsync(TRecord record)
    {
        var id = RecordUtilities.GetIdentity(record);
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        if (modalDialog is not null && this.UIEntityService.ViewForm is not null)
        {
            await modalDialog.ShowAsync(this.UIEntityService.ViewForm, options);
            this.StateHasChanged();
        }
        else
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/view/{id}");
    }

    protected virtual Task OnDashboardAsync(TRecord record)
    {
        var id = RecordUtilities.GetIdentity(record);
        this.NavManager.NavigateTo($"{this.UIEntityService.Url}/dash/{id}");
        return Task.CompletedTask;
    }

    protected Task LogErrorMessageAsync(string message)
    {
        Logger.LogError(message);
        return Task.CompletedTask;
    }

    protected void LogErrorMessage(string message)
    {
        Logger.LogError(message);
    }

    public async ValueTask DisposeAsync()
    {
        _disposable?.Dispose();

        if (this.Presenter is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
    }
}
