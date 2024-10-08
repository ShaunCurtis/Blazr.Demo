﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;
public interface IEditPresenter<TRecord, TKey, TEditContext>
    where TRecord : class, new()
    where TEditContext : IRecordEditContext<TRecord>, new()
{
    public IDataResult LastDataResult { get; }
    public EditContext EditContext { get; }
    public TEditContext RecordEditContext { get; }
    public bool IsNew { get; }
    public bool IsInvalid { get; }

    public Task<IDataResult> SaveItemAsync();
}
