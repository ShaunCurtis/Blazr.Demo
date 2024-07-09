/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Presentation.Toasts;

/// <summary>
/// Manages the Toasts in a Vanilla Bootstrap UI.
/// </summary>
public class VanillaUIToastService : IAppToastService, IAppToastViewService
{
    private readonly List<Toast> _toasts = new();
    private TimeSpan _defaultTimeOut = TimeSpan.FromSeconds(20);
    
    /// <summary>
    /// Event raised when thw Toast list changes
    /// </summary>
    public event EventHandler? ToastsChanged;

    /// <summary>
    /// Gets the list of active Toasts
    /// </summary>
    public IEnumerable<Toast> Toasts
    {
        get
        {
            ClearExpiredMessages();
            return _toasts.AsEnumerable();
        }
    }

    /// <summary>
    /// Adds an Error Toast to the Service
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="timeout"></param>
    public void ShowError(string Message, TimeSpan? timeout = null)
    {
        _toasts.Add(new(Message, ToastType.Error, timeout ?? _defaultTimeOut));
        this.ToastsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Adds an Success Toast to the Service
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="timeout"></param>
    public void ShowSuccess(string Message, TimeSpan? timeout = null)
    {
        _toasts.Add(new(Message, ToastType.Success, timeout ?? _defaultTimeOut));
        this.ToastsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Adds a Warning Toast to the Service
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="timeout"></param>
    public void ShowWarning(string Message, TimeSpan? timeout = null)
    {
        _toasts.Add(new(Message, ToastType.Warning, timeout ?? _defaultTimeOut));
        this.ToastsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Dimisses a Toast i.e. removes it from the Toast list
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    public void DismissToast(Guid id, object? token = null)
    {
        var toast = _toasts.FirstOrDefault(item => item.Uid == id);
        if (toast != null)
        {
            _toasts.Remove(toast);
            this.ToastsChanged?.Invoke(token ?? this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets the TimeSpan to the next Toast to expire
    /// Returns TimeSpan.Zero if there are currently no toasts
    /// </summary>
    /// <returns></returns>
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
