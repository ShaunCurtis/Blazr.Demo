/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public readonly record struct CommandRequest<TRecord>(TRecord Item, CommandState State, CancellationToken Cancellation = new());

public readonly record struct CommandAPIRequest<TRecord>
{
    public TRecord? Item { get; init; }
    public int CommandIndex { get; init; }

    public CommandAPIRequest() { }

    public static CommandAPIRequest<TRecord> FromRequest(CommandRequest<TRecord> command)
        => new()
        {
            Item = command.Item,
            CommandIndex = command.State.Index
        };

    public CommandRequest<TRecord> ToRequest(CancellationToken? cancellation = null)
        => new()
        {
            Item = this.Item ?? default!,
            State = CommandState.GetState(this.CommandIndex),
            Cancellation = cancellation ?? CancellationToken.None
        };
}

public readonly record struct CommandState
{
    public int Index { get; private init; } = 0;
    public string Value { get; private init; } = "None";

    public CommandState() { }

    public CommandState(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public static CommandState None = new CommandState(0, "None");
    public static CommandState Add = new CommandState(1, "Add");
    public static CommandState Update = new CommandState(2, "Update");
    public static CommandState Delete = new CommandState(-1, "Delete");

    public static CommandState GetState(int index)
        => (index) switch
        {
            1 => CommandState.Add,
            2 => CommandState.Update,
            -1 => CommandState.Delete,
            _ => CommandState.None,
        };
}
