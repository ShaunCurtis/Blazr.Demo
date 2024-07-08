/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Presentation.Toasts;

public record Toast(string Message, ToastType Type, TimeSpan Timeout)
{
    public DateTimeOffset TimeStamp { get; } = DateTimeOffset.Now;
    public bool TimedOut => this.TimeStamp.AddSeconds(this.Timeout.Seconds) < DateTimeOffset.Now;
    public TimeSpan TimeOut => DateTimeOffset.Now - this.TimeStamp;
}

public enum ToastType
{
    Success,
    Warning,
    Error
}
public interface IAppToastViewService
{
    public event EventHandler? ToastsChanged;
    public IEnumerable<Toast> Toasts { get; }
    public TimeSpan NextToastTimeOut();
}

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

    public TimeSpan NextToastTimeOut()
    {
        this.ClearExpiredMessages();
        if (_toasts.Count == 0)
            return TimeSpan.Zero;

        return _toasts.Min(item => item.TimeOut);
    }

    private void ClearExpiredMessages()
    {
        var expiredToasts = _toasts.Where(item => item.TimedOut).ToList();
        foreach (var toast in expiredToasts)
            _toasts.Remove(toast);
    }
}
