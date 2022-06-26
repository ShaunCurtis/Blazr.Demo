/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface INotificationService<TService>
    where TService : class, IEntityService
{
    public event EventHandler? ListUpdated;
    public event EventHandler<PagingEventArgs>? ListPaged;
    public event EventHandler<RecordEventArgs>? RecordChanged;

    public void NotifyListUpdated(object? sender);

    public void NotifyListPaged(object? sender, int page);

    public void NotifyRecordChanged(object? sender, Guid Id);
}
