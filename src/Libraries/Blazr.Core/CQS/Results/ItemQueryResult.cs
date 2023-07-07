/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public sealed record ItemQueryResult<TRecord> : IDataResult
{
    public TRecord? Item { get; init;} 
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;

    private ItemQueryResult() { }

    public static ItemQueryResult<TRecord> Success(TRecord Item, string? message = null)
        => new ItemQueryResult<TRecord> { Successful=true, Item= Item, Message= message ?? string.Empty };

    public static ItemQueryResult<TRecord> Failure(string message)
        => new ItemQueryResult<TRecord> { Message = message};
}
