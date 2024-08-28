/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

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

