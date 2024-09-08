# Results

Consider this classic pattern:

```csharp
private async ValueTask<TRecord?> ExecuteQueryAsync<TRecord>(QueryRequest request)
```

We return either an instance of `TRecord` or null.

It smells.  What does `null` mean?  

1. No record exists with that Id.
2. The API call timed out.
3. Your request was bad.
4. Bog off, I'm busy.

Unless you're putting the information somewhere else for the called to retrieve, you don't know.  You can raise various exceptions to trap, but that's expensive and excessive.

The answer is to return a result object.  That contains a status flag, the data and a message.

First a very general base result interface to handle status information regardless of the type of data.

```csharp
public interface IDataResult
{
    public bool Successful { get; }
    public string Message { get; }
}
```

And an implementation:

```csharp
public sealed record DataResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string? Message { get; init; }

    internal DataResult() { }

    public static DataResult Success(string? message = null)
        => new DataResult { Successful = true, Message= message };

    public static DataResult Failure(string message)
        => new DataResult { Message = message};

    public static DataResult Create(bool success, string? message = null)
        => new DataResult { Successful = success, Message = message };
}
```

Next we can add a result:

```csharp
public sealed record DataResult<TData> : IDataResult
{
    public TData? Item { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    internal DataResult() { }

    public static DataResult<TData> Success(TData Item, string? message = null)
        => new DataResult<TData> { Successful = true, Item = Item, Message = message };

    public static DataResult<TData> Failure(string message)
        => new DataResult<TData> { Message = message };
}
```

## Coming Soon

Net 9 will hopefully change how we handle returns though a new launuage feature called *Discriminated Unions*.  Search for *Discriminated Unions* for more information.  