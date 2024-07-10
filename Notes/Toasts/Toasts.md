# Toasts

Toasts [the Snackbar in MubBlazor] are implemented through a common interface.  This abstracts consumers, such as generic presenters from the individual implementations.

The base interface looks like this:

```csharp
public interface IAppToastService
{
    public void ShowWarning(string Message, TimeSpan? timeout = null);
    public void ShowSuccess(string Message, TimeSpan? timeout = null);
    public void ShowError(string Message, TimeSpan? timeout = null);
}
```

## FluentUI implementation

This injects the registered FluentUI `IToastService` and then makes calls into that service to show toasts.  It uses the default FluentUI timeout.

```csharp
public class FluentUIToastService : IAppToastService
{
    private IToastService _toastService;

    public FluentUIToastService(IToastService toastService)
    {
        _toastService = toastService;
    }

    public void ShowError(string message, TimeSpan? timeout = null)
    {
        _toastService.ShowError(message);
    }

    public void ShowSuccess(string message, TimeSpan? timeout = null)
    {
        _toastService.ShowSuccess(message);
    }

    public void ShowWarning(string message, TimeSpan? timeout = null)
    {
        _toastService.ShowWarning(message);
    }
}
```

## MudBlazor Snackbar Implementation

This injects the registered `ISnackbar` and then makes calls into that service to add toasts.  It uses the default snackbar timeout.

```csharp
public class MudBlazorUIToastService : IAppToastService
{
    private ISnackbar _snackbar;

    public MudBlazorUIToastService(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    public void ShowError(string message, TimeSpan? timeout = null)
    {
        _snackbar.Add(message, Severity.Error, c => c.SnackbarVariant = Variant.Filled);
    }

    public void ShowSuccess(string message, TimeSpan? timeout = null)
    {
        _snackbar.Add(message, Severity.Success, c => c.SnackbarVariant = Variant.Filled);
    }

    public void ShowWarning(string message, TimeSpan? timeout = null)
    {
        _snackbar.Add(message, Severity.Warning, c => c.SnackbarVariant = Variant.Filled);
    }
}
```

## Vanilla Bootstrap Implementation

You can't use the Bootstrap code as-is.  It needs to be *Blazored*.

First a `Toast`

```csharp
public record Toast(string Message, ToastType Type, TimeSpan Timeout)
{
    public Guid Uid { get; } = Guid.NewGuid();
    public DateTimeOffset TimeStamp { get; } = DateTimeOffset.Now;
    public DateTimeOffset ExpiredTimeStamp => this.TimeStamp.Add(this.Timeout);
    public TimeSpan TimeLeftToExpire => this.ExpiredTimeStamp.Subtract(DateTimeOffset.Now);
    public bool TimedOut => DateTimeOffset.Now > this.ExpiredTimeStamp;
}

public enum ToastType
{
    Success,
    Warning,
    Error
}
```

And then an interface for the UI component displaying the toasts.

```csharp
public interface IAppToastViewService
{
    public event EventHandler? ToastsChanged;
    public IEnumerable<Toast> Toasts { get; }
    public TimeSpan NextToastTimeOut();
    public void DismissToast(Guid id, object? token = null);
}
```

The vanilla toast service:

```csharp
public class VanillaUIToastService : IAppToastService, IAppToastViewService
{
    private readonly List<Toast> _toasts = new();
    private TimeSpan _defaultTimeOut = TimeSpan.FromSeconds(20);
    
    public event EventHandler? ToastsChanged;

    public IEnumerable<Toast> Toasts
    {
        get
        {
            ClearExpiredMessages();
            return _toasts.AsEnumerable();
        }
    }

    public void ShowError(string Message, TimeSpan? timeout = null)
    {
        _toasts.Add(new(Message, ToastType.Error, timeout ?? _defaultTimeOut));
        this.ToastsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ShowSuccess(string Message, TimeSpan? timeout = null)
    {
        _toasts.Add(new(Message, ToastType.Success, timeout ?? _defaultTimeOut));
        this.ToastsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ShowWarning(string Message, TimeSpan? timeout = null)
    {
        _toasts.Add(new(Message, ToastType.Warning, timeout ?? _defaultTimeOut));
        this.ToastsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DismissToast(Guid id, object? token = null)
    {
        var toast = _toasts.FirstOrDefault(item => item.Uid == id);
        if (toast != null)
        {
            _toasts.Remove(toast);
            this.ToastsChanged?.Invoke(token ?? this, EventArgs.Empty);
        }
    }

    public TimeSpan NextToastTimeOut()
    {
        this.ClearExpiredMessages();
        if (_toasts.Count == 0)
            return TimeSpan.Zero;

        var value = _toasts.Min(item => item.TimeLeftToExpire);
        // add a buffer to ensure things have timed out.  There's a +- 15 ms on the system time.
        return value.Add(TimeSpan.FromMilliseconds(100));
    }

    private void ClearExpiredMessages()
    {
        var expiredToasts = _toasts.Where(item => item.TimedOut).ToList();
        foreach (var toast in expiredToasts)
            _toasts.Remove(toast);
    }
}
```

And finally the UI component.

```csharp
@using System.Diagnostics
@inject IAppToastService ToastService
@implements IDisposable

<div class="toast-container position-fixed top-0 end-0 p-3" style="z-index:11;">

    @foreach (var toast in _toastViewService.Toasts)
    {
        <div class="toast show @this.ToastCss(toast.Type)" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    @toast.Message
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" @onclick="() => this.DismissToast(toast.Uid)"></button>
            </div>
        </div>
    }
</div>

@code {
    private IAppToastViewService _toastViewService = default!;
    private Timer? _timer;
    private object _token = new();

    protected override void OnInitialized()
    {
        if (ToastService is IAppToastViewService appToastViewService)
            _toastViewService = appToastViewService;

        ArgumentNullException.ThrowIfNull(_toastViewService);

        _toastViewService.ToastsChanged += this.OnToastsChanged;
        this.OnTimerExpired(null);
    }

    private string ToastCss(ToastType toastType) => toastType switch
    {
        ToastType.Success => "text-white bg-success",
        ToastType.Warning => "text-white bg-warning",
        ToastType.Error => "text-white bg-danger",
        _ => "text-white bg-primary"
    };

    private void OnToastsChanged(object? sender, EventArgs e)
    {
        if (sender != _token)
        {
            this.OnTimerExpired(null);
            this.InvokeAsync(StateHasChanged);
        }
    }

    private void OnTimerExpired(object? state)
    {
        // Clear down the current timer if it exists
        if (_timer is not null)
        {
            _timer.Dispose();
            _timer = null;
        }

        // Get the next timeout
        var timeOut = _toastViewService.NextToastTimeOut();

        // If we have one, add a new one time timer
        if (timeOut != TimeSpan.Zero)
            _timer = new Timer(this.OnTimerExpired, null, timeOut, Timeout.InfiniteTimeSpan);
 
        // Update the UI - a Toast has beed added or removed
        this.InvokeAsync(StateHasChanged);
    }

    private void DismissToast(Guid id)
    {
        _toastViewService.DismissToast(id, _token);
    }

    public void Dispose()
    {
        _toastViewService.ToastsChanged -= this.OnToastsChanged;
    }
}
```

There's no revolving timer service.  The Toast list is pruned each time you get the list.  The UI component schedules updates based on the expiration period to the next toast, or resets and renders if a new toast is added via the registered event handler.