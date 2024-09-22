/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public readonly record struct DataResult : IDataResult
{ 
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public DataResult() { }

    public static DataResult Success(string? message = null)
        => new DataResult { Successful = true, Message= message };

    public static DataResult Failure(string message)
        => new DataResult { Message = message};
}

public readonly record struct DataResult<TData> : IDataResult
{
    public TData? Item { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public DataResult() { }

    public static DataResult<TData> Success(TData Item, string? message = null)
        => new DataResult<TData> { Successful = true, Item = Item, Message = message };

    public static DataResult<TData> Failure(string message)
        => new DataResult<TData> { Message = message };
}

