# Group Entities And Notification

Data and objects in the solution are grouped into *GroupEntities*: you will see these in the folder structure.  A *Group Entity* is a collection of related objects.  For example the Core domain `Invoice` group entity contains all the data classes and services associated with Invoicing.

Each Group Entity has a `IEntityService`.  This class is used as a group identity in defining services.

```csharp
public interface IEntityService { }
```

The Invoice concrete implementation.

```csharp
public class InvoiceEntityService : IEntityService {}
```

Why do we need these?  They don't do anything.

Consider a data edit.  The user clicks on an edit putton on a row in the list form.  It pops up a edit dialog and the user changes the data and exits.  How does the list form know that a value in the list has changed?  It needs a notification.

The Notification Service definition:

```csharp
public interface INotificationService<TEntityService>
    where TEntityService : class, IEntityService
{
    public event EventHandler? ListChanged;
    public event EventHandler<RecordChangedEventArgs>? RecordChanged;

    public void NotifyListChanged(object? sender);
    public void NotifyRecordChanged(object? sender, object record);
```

And the implementation:

```csharp
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
```

We can define a Notification Service for each Group Entity like this:

```csharp
services.AddScoped<INotificationService<InvoiceEntityService>, NotificationService<InvoiceEntityService>>();
```

No concrete class defined.  The edit presenter injects the instance defined in DI and calls `NotifyRecordChanged` to raise the `RecordChanged` event.  The list presenter injects the same notification instance and registers a handler on the `RecordChanged` event.  It does whatever it needs to do and calls `NotifyListChanged`....

Here are the service definitions for the List and Edit presenters,

```csharp
services.AddScoped<IListPresenter<Invoice, InvoiceEntityService>, ListPresenter<Invoice, InvoiceEntityService>>();
services.AddTransient<IEditPresenter<Invoice, InvoiceEditContext>, EditPresenter<Invoice, InvoiceEntityService, InvoiceEditContext>>();

```



 