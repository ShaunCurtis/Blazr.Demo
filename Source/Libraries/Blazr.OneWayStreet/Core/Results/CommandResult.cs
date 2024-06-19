/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public sealed record CommandResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string? Message { get; init; }
    public object? KeyValue { get; init; }

    public CommandResult() { }

    public static CommandResult Success(string? message = null)
        => new CommandResult { Successful = true, Message= message };

    public static CommandResult SuccessWithKey(object keyValue, string? message = null)
        => new CommandResult { Successful = true, KeyValue = keyValue, Message = message };

    public static CommandResult Failure(string message)
        => new CommandResult { Message = message};
}

public sealed record CommandAPIResult<TKey>
{
    public bool Successful { get; init; }
    public string? Message { get; init; }
    public TKey KeyValue { get; init; } = default!;

    public CommandAPIResult() { }

    public CommandResult ToCommandResult()
        => new()
        {
            Successful = Successful,
            Message = Message,
            KeyValue = KeyValue
        };
}
