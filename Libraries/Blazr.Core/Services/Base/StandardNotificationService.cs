/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class StandardNotificationService<TService> : INotificationService<TService>
    where TService : class, IEntityService
{
    public event EventHandler? ListUpdated;
    public event EventHandler<PagingEventArgs>? ListPaged;
    public event EventHandler<RecordEventArgs>? RecordChanged;

    public void NotifyListUpdated(object? sender)
        => this.ListUpdated?.Invoke(this, EventArgs.Empty);

    public void NotifyListPaged(object? sender, int page)
        => this.ListPaged?.Invoke(sender, new PagingEventArgs(page));

    public void NotifyRecordChanged(object? sender, Guid Id)
    {
        if (Id != Guid.Empty)
            this.RecordChanged?.Invoke(sender, new RecordEventArgs(Id));
    }
}
