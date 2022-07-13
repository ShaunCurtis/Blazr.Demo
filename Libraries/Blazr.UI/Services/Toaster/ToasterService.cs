/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Timers;

namespace Blazr.UI;

public class ToasterService : IDisposable
{
    private readonly List<Toast> _toastList = new List<Toast>();
    private System.Timers.Timer _timer = new System.Timers.Timer();

    public event EventHandler? ToasterChanged;

    public event EventHandler? ToasterTimerElapsed;

    public bool HasToasts => _toastList.Count > 0;

    public ToasterService()
    {
        AddToast(new Toast { Title = "Welcome Toast", Message = "Welcome to this Application.  I'll disappear after 5 seconds.", TTD = DateTimeOffset.Now.AddSeconds(3) });
        _timer.Interval = 2000;
        _timer.AutoReset = true;
        _timer.Elapsed += this.TimerElapsed;
        _timer.Start();
    }

    public List<Toast> GetToasts()
    {
        ClearTTDs();
        return _toastList;
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    { 
        this.ClearTTDs();
        this.ToasterTimerElapsed?.Invoke(this, EventArgs.Empty);
    }

    public void AddToast(Toast toast)
    {
        _toastList.Add(toast);
        // only raise the ToasterChanged event if it hasn't already been raised by ClearTTDs
        if (!this.ClearTTDs())
            this.ToasterChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearToast(Toast toast)
    {
        if (_toastList.Contains(toast))
        {
            _toastList.Remove(toast);
            // only raise the ToasterChanged event if it hasn't already been raised by ClearTTDs
            if (!this.ClearTTDs())
                this.ToasterChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool ClearTTDs()
    {
        var toastsToDelete = _toastList.Where(item => item.IsBurnt).ToList();
        if (toastsToDelete is not null && toastsToDelete.Count > 0)
        {
            toastsToDelete.ForEach(toast => _toastList.Remove(toast));
            this.ToasterChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        if (_timer is not null)
        {
            _timer.Elapsed += this.TimerElapsed;
            _timer.Stop();
        }
    }
}

