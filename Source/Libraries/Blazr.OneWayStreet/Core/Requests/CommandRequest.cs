/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public record struct CommandRequest<TRecord>(TRecord Item, CommandState State, CancellationToken Cancellation = new());

public record struct CommandAPIRequest<TRecord>
{
    public TRecord? Item { get; init; }
    public CommandState State { get; init; } = CommandState.None;

    public CommandAPIRequest() { }

    public static CommandAPIRequest<TRecord> FromRequest(CommandRequest<TRecord> command)
        => new()
        {
            Item = command.Item,
            State = command.State,
        };

    public CommandRequest<TRecord> ToRequest(CancellationToken? cancellation = null)
        => new()
        {
            Item = this.Item ?? default!,
            State = this.State,
            Cancellation = cancellation ?? CancellationToken.None
        };
}

public record CommandState
{
    public int Index { get; private init; }
    public string Value { get; private init; }

    internal CommandState(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public static CommandState None = new CommandState(0, "None");
    public static CommandState Add = new CommandState(1, "Add");
    public static CommandState Update = new CommandState(2, "Update");
    public static CommandState Delete = new CommandState(-1, "Delete");
}