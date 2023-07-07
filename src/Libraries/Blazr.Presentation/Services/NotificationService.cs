/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public class NotificationService<TEntityService> : INotificationService<TEntityService>
    where TEntityService : class, IEntityService
{
    public event EventHandler? ListChanged;
    public event EventHandler<RecordChangedEventArgs>? RecordChanged;

    public void NotifyListChanged(object? sender)
        => this.ListChanged?.Invoke(sender, EventArgs.Empty);

    public void NotifyRecordChanged(object? sender, object record)
        => this.RecordChanged?.Invoke(sender, RecordChangedEventArgs.Create(record));
}
