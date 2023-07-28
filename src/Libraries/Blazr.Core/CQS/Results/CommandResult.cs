/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public sealed record CommandResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string? Message { get; init; }

    private CommandResult() { }

    public static CommandResult Success(string? message = null)
        => new CommandResult { Successful = true, Message= message };

    public static CommandResult Failure(string message)
        => new CommandResult { Message = message};
}
