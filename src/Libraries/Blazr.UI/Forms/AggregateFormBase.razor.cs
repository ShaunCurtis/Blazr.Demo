/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public abstract partial class AggregateFormBase<TEntityService> : BlazrControlBase
    where TEntityService : class, IEntityService
{
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IUIEntityService<TEntityService> UIEntityService { get; set; } = default!;
    [Inject] protected INotificationService<TEntityService> NotificationService { get; set; } = default!;

    [Parameter] public Guid Uid { get; set; }
    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }

    protected string ExitUrl { get; set; } = "/";

    protected virtual Task OnExitAsync()
    {
        if (this.ModalDialog is null)
            this.NavManager.NavigateTo(this.ExitUrl);

        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }
}
