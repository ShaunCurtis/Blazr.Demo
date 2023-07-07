/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public interface INotificationService<TEntityService>
    where TEntityService : class, IEntityService
{
    public event EventHandler? ListChanged;
    public event EventHandler<RecordChangedEventArgs>? RecordChanged;

    public void NotifyListChanged(object? sender);
    public void NotifyRecordChanged(object? sender, object record);
}
