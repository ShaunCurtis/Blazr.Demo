﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface ICQSDataBroker
{
    public ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordListQuery<TRecord> query) where TRecord : class, new();

    public ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IFilteredListQuery<TRecord> query) where TRecord : class, new();

    public ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordGuidKeyQuery<TRecord> query) where TRecord : class, new();

    public ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new();

    public ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new();

    public ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new();

    public ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new();

    public ValueTask<object> ExecuteAsync<TRecord>(object query);
}