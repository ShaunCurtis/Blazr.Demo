# ReadOnly By Default

One of the most common sources of bugs in code is silent mutation: inadvertantly changing values in an object.

The most obvious way to tame this mutation beast is to consider the purpose of each object when you create it.  If there's no reason for mutation, make it readonly.  For many the default setting is `class`, they never consider anything else.

Don't fall into that trap.  My default is a record.  And then the the struct or a class decision.  I use `readonly record struct` a lot since it's introduction.

Consider the data pipeline Command request object.

A common class based implementation would be:

```csharp
public class CommandRequest<TRecord>
{ 
    public TRecord Item { get; set; }
    public CommandState State { get; set; };
    public CancellationToken Cancellation { get; set; } = new();

    public CommandRequest(TRecord item, CommandState state, CancellationToken cancellationToken = new())
    {
        Item = item;
        State = state;
        Cancellation = cancellationToken;
    }
}
```

You can define the same object as a `readonly record struct`.

```csharp
public readonly record struct CommandRequest
    <TRecord>(TRecord Item, 
    CommandState State, 
    CancellationToken Cancellation = new());
```

It's an immutable stack based value object with record semantics.
Note that `TRecord` and `CommandState` are also immutable objects.

The advantages of mskeing this switch are:

1. Quicker - structs are stack based objects.
2. Easier to copy - use `with`.
3. Easier to compare - value based equality semantics.



