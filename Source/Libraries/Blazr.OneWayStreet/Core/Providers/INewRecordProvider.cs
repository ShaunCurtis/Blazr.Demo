/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

/// <summary>
/// All Concrete implementations should be registered as Singletons to ensure unique new record Ids
/// are generated for volatile records in composite objects
/// </summary>
/// <typeparam name="TRecord"></typeparam>
public interface INewRecordProvider<TRecord>
    where TRecord : new()
{
    public TRecord NewRecord();
}

public interface INewRecordProvider
{
    public TRecord NewRecord<TRecord>()
            where TRecord : new();
}
