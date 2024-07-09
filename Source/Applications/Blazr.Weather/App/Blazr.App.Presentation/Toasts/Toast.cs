/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Presentation.Toasts;

/// <summary>
/// Record representing a Toast in the Bootstrap UI enviroment
/// </summary>
/// <param name="Message"></param>
/// <param name="Type"></param>
/// <param name="Timeout"></param>
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
