/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record CommandResult
{
    public Guid NewId { get; init; } = Guid.Empty;

    public bool Success { get; init; } = false;

    public string Message { get; init; } = string.Empty;

    public CommandResult() { }

    public static CommandResult Failure(string message)
        => new CommandResult { Message= message };

    public static CommandResult Successful(string? message = null)
        => new CommandResult {Success = true, Message = message ?? "The command completed successfully" };

    public static CommandResult Successful(Guid? Uid= null,  string? message =null)
        => new CommandResult { NewId = Uid ?? Guid.Empty, Message = message ?? "The command completed successfully", Success= true };
}
