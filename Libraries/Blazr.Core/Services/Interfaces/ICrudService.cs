﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface ICrudService<TRecord, TEditRecord>
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
{

    public TRecord? Record { get; }

    public TEditRecord EditModel { get; }

    public bool IsNewRecord { get; }

    public string? Message { get; }

    public ValueTask GetRecordAsync(Guid Id);

    public ValueTask GetNewRecordAsync(TRecord? record);

    public ValueTask<bool> AddRecordAsync();

    public ValueTask<bool> UpdateRecordAsync();

    public ValueTask<bool> DeleteRecordAsync();
}
