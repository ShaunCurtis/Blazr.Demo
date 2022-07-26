/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record CommandResult
{
    public Guid NewId { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; }

    public CommandResult() { }

    public CommandResult( Guid newId, bool success, string message)
    {
        NewId = newId;
        Success = success;
        Message = message;
    }
    public CommandResult(bool success)
    {
        NewId = Guid.Empty;
        Success = success;
        Message = "The command completed successfully";
    }
}
