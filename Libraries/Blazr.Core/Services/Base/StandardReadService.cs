﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class StandardReadService<TRecord, TService>
    : IReadService<TRecord>
    where TRecord : class, new()
    where TService : class, IEntityService
{
    protected ICQSDataBroker DataBroker;
    protected INotificationService<TService> Notifier;

    public TRecord? Record { get; private set; }

    public string? Message { get; protected set; }

    public bool HasRecord => this.Record is not null;

    public StandardReadService(ICQSDataBroker dataBroker, INotificationService<TService> notifier)
    {
        this.DataBroker = dataBroker;
        Notifier = notifier;
    }

    public async ValueTask GetRecordAsync(Guid Id)
    {
        this.Message = String.Empty;
        var result = await this.DataBroker.ExecuteAsync<TRecord>(new RecordGuidKeyQuery<TRecord>(Id));

        if (result.Success && result.Record is not null)
        {
            this.Record = result.Record;
            return;
        }
        this.Record = null;
        this.Message = $"Failed to retrieve the record with Id - {Id.ToString()}";
    }
}

