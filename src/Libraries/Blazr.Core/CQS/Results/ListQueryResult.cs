/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public sealed record ListQueryResult<TRecord> : IDataResult
{
    public IEnumerable<TRecord> Items { get; init;} = Enumerable.Empty<TRecord>();  
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;
    public long TotalCount { get; init; }

    private ListQueryResult() { }

    public static ListQueryResult<TRecord> Success(IEnumerable<TRecord> Items, long totalCount, string? message = null)
        => new ListQueryResult<TRecord> {Successful=true,  Items= Items, TotalCount = totalCount, Message= message ?? string.Empty };

    public static ListQueryResult<TRecord> Failure(string message)
        => new ListQueryResult<TRecord> { Message = message};
}
