﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface ICustomListQuery<TRecord>
    : ICQSRequest<ValueTask<ListProviderResult<TRecord>>>
    where TRecord : class, new()
{
    public ListProviderRequest<TRecord> Request { get; }

    public Func<TRecord, bool>? FilterExpression { get; }
}