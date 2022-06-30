/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class StandardCrudService<TRecord, TEditRecord, TService>
    : ICrudService<TRecord, TEditRecord>
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
    where TService : class, IEntityService
{
    protected readonly ICQSDataBroker DataBroker;
    protected INotificationService<TService> Notifier;

    public TRecord? Record { get; private set; } = new TRecord();

    public TEditRecord EditModel { get; private set; } = new TEditRecord();

    public bool IsNewRecord => this.EditModel.IsNew;

    public string? Message { get; protected set; }

    public StandardCrudService(ICQSDataBroker dataBroker, INotificationService<TService> notifier)
    {
        this.DataBroker = dataBroker;
        this.Notifier = notifier;
    }

    public async ValueTask GetRecordAsync(Guid Id)
    {
        this.Message = String.Empty;
        if (Id != Guid.Empty)
        {
            var result = await this.DataBroker.ExecuteAsync<TRecord>(new RecordQuery<TRecord>(Id));

            if (result.Success && result.Record is not null)
            {
                this.Record = result.Record;
                this.EditModel.Load(this.Record);
                return;
            }

            this.Message = $"Unable to retrieve record with Id : {Id.ToString()}";
            return;
        }
        else
            await GetNewRecordAsync(null);
    }

    public ValueTask GetNewRecordAsync(TRecord? record)
    {
        this.Record = record ?? new TRecord();
        this.EditModel.Load(this.Record);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<bool> AddRecordAsync()
    {
        this.Message = String.Empty;

        if (!EditModel.IsNew)
        {
            this.Message = "You can't add an existing record";
            return false;
        }

        this.Record = EditModel.AsNewRecord;

        var result = await this.DataBroker.ExecuteAsync<TRecord>(new AddRecordCommand<TRecord>(this.Record));

        if (!result.Success)
        {
            this.Message = "Failed to add the record";
            return false;
        }

        this.EditModel.Load(this.Record);
        this.NotifyChange(EditModel.Id);
        return true;
    }

    public async ValueTask<bool> UpdateRecordAsync()
    {
        this.Record = EditModel.Record;
        var result = await this.DataBroker.ExecuteAsync<TRecord>(new UpdateRecordCommand<TRecord>(this.Record));

        if (!result.Success)
        {
            this.Message = "Failed to update the record";
            return false;
        }

        this.EditModel.Load(this.Record);
        this.NotifyChange(EditModel.Id);
        return true;
    }

    public async ValueTask<bool> DeleteRecordAsync()
    {
        if (this.Record is null)
        {
            this.Message = "No record to delete";
            return false;
        }

        var id = GetRecordId<TRecord>(this.Record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(new DeleteRecordCommand<TRecord>(this.Record));

        if (!result.Success)
        {
            this.Message = "Failed to update the record";
            return false;
        }
        this.Record = null;
        this.NotifyChange(id);
        return true;
    }

    private void NotifyChange(Guid? Uid = null)
    {
        if (Uid is not null)
            this.Notifier?.NotifyRecordChanged(this, Uid ?? Guid.Empty);

        this.Notifier?.NotifyListUpdated(this.Record);
    }

    private static Guid GetRecordId<T>(T? record) where T : class, new()
    {
        if (record == null)
            return Guid.Empty;

        var instance = new T();
        var prop = instance.GetType()
            .GetProperties()
            .FirstOrDefault(prop => prop.GetCustomAttributes(false)
                .OfType<KeyAttribute>()
                .Any());

        if (prop != null)
        {
            var value = prop.GetValue(record);
            if (value is not null)
                return (Guid)value;
        }
        return Guid.Empty;
    }
}
