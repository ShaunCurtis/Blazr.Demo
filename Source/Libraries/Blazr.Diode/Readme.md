# Blazr.Diode

Diode is a coding pattern for dealing with complex domain entities in C#.  It was inspired by the Flux/Redux pattern.

It provides:

 - a wrapper - the `DiodeContext` for managed mutation of an object. 
 - state management [`DiodeState`] for any managed object.
 - notification when a mutation takes place.

It doesn't provide a store or single point dispatcher.  The store and dispatcher functionality should be provided by the root domain entity container.

## Class vs Record

Diode relies on the managed object being a readonly object.  In modern C# a record, but it could be a simple class with `init` constructors.  The advantage of using a record is that cloning and equality checking are simple.  Diode ius designed to plug into a readonly data pipeline - in my case my `OneWayStreet` library.   

### DiodeState

Diode state is implemented as a readonly struct with an internal constructor.  Obtain a State object through static readonly properties.

```csharp
public readonly struct DiodeState : IEquatable<DiodeState>
{
    public int Index { get; private init; }
    public string Value { get; private init; }

    internal DiodeState(int index, string value)
    {
        Index = index;
        Value = value; 
    }

    public static DiodeState Clean = new DiodeState(0, "Clean");
    public static DiodeState New = new DiodeState(1, "New");
    public static DiodeState Modified = new DiodeState(2, "Modified");
    public static DiodeState Deleted = new DiodeState(-1, "Deleted");

    //... IEquatable implementation
}
```

### DiodeContext

The `DiodeContext` is the management wrapper.  A `DiodeContext` is defined with the `TRecord` that is being managed and a unique  `TIdentity` key. 

```csharp
public class DiodeContext<TIdentity, TRecord>
    where TRecord : class, IDiodeRecord<TIdentity>, new()
```

The public properties are all readonly.  Note that `Item` will point to the current copy of the managed object.  Either don't keep a local reference to it, or refresh the local reference whwn you use it.  `StateChanges` is an incrementing counter tracking the number of mutations that have been applied. 

```csharp
    public TIdentity Id {get;}
    public TRecord Item {get;}
    public DiodeState State { get; }
    public int StateChanges {get;}
    public event EventHandler<DiodeEventArgs>? StateHasChanged;

```

There are three actions you can apply.  

 - `Update` applies the provided `DiodeMutationDelegate` to the managed object.  It will replace the managed object with the new mutated copy and update the state.
 - `Delete` updates the state.
 - `Persisted` resets the state.  Call `Persisted` when the objct had been saved to it's permenant store.


```csharp
    public DiodeResult Update(DiodeMutationDelegate<TIdentity, TRecord> mutation, object? sender = null) { }
    public DiodeResult Delete(object? sender = null) { }
    public void Persisted(object? sender = null) { }
}
```

The constructor is internal, so `DiodeContext` can only be constructed using two static constructors.  They ensure the correct initial state. 

```csharp
internal DiodeContext(TRecord item, DiodeState? state = null)  { }

public static DiodeContext<TIdentity, TRecord> CreateNew(TRecord item)
{
    return new(item, DiodeState.New);
}

public static DiodeContext<TIdentity, TRecord> CreateClean(TRecord item)
{
    return new(item, DiodeState.Clean);
}
```

## IDiodeRecord

Diode records must implement `IDiodeRecord`.  `Id` is used to identify DiodeContexts within a list.

```csharp
public interface IDiodeRecord<TIdentity>
{
    TIdentity Id { get; }
}
```

## DiodeEventArgs

State change events in Diode return a `DiodeEventArgs` object;

```csharp
public class DiodeEventArgs : EventArgs
{
    public object? Item { get; set; }
    public DiodeState State { get; set; }

    public DiodeEventArgs(object? item, DiodeState state)
    {
        Item = item;
        State = state;
    }
}
```

## Diode Results

Diode actions return a `DiodeResult` or a `DiodeMutationResult`.

```csharp
public record DiodeResult
{
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static DiodeResult Success(string? message = null)
        => new DiodeResult() { Successful = true, Message = message };

    public static DiodeResult Failure(string message)
        => new DiodeResult() { Successful = false, Message = message };
}
```

```csharp
public record DiodeMutationResult<T> : DiodeResult
        where T : class
{
    public T Item { get; init; } = default!;

    public static DiodeMutationResult<T> Success(T item, string? message = null)
        => new DiodeMutationResult<T>() { Successful = true, Item = item, Message = message };

    public static DiodeMutationResult<T> Failure(string message, T? item = null)
        => new DiodeMutationResult<T>() { Successful = false, Message = message, Item = item! };
}
```

## DiodeMutationDelegate

Diode record mutations are defined by a `DiodeMutationDelegate`:

```csharp
public delegate DiodeMutationResult<TRecord> DiodeMutationDelegate<TIdentity, TRecord>(DiodeContext<TIdentity, TRecord> item)
    where TRecord : class, IDiodeRecord<TIdentity>, new();
```



