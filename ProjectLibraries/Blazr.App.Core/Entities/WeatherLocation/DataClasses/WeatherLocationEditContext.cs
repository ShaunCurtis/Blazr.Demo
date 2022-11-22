/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherLocationEditContext
    : RecordEditContextBase<DboWeatherLocation>, IMessageCollectionValidation, IAuthRecord
{
    private Guid _newId = Guid.NewGuid();

    private Guid _uid = Guid.Empty;
    public override Guid Uid
    {
        get => _uid;
        set => UpdateifChangedAndNotify(ref _uid, value, this.BaseRecord.Uid, WeatherLocationConstants.Uid);
    }

    private Guid _ownerId = Guid.Empty;
    public Guid OwnerId
    {
        get => _ownerId;
        set => UpdateifChangedAndNotify(ref _ownerId, value, this.BaseRecord.OwnerId, WeatherLocationConstants.OwnerId);
    }

    private string _location = string.Empty;
    public string Location
    {
        get => _location;
        set => UpdateifChangedAndNotify(ref _location, value, this.BaseRecord.Location, WeatherLocationConstants.Location);
    }

    public WeatherLocationEditContext()
    {
        var rec = new DboWeatherLocation();
        this.Load(rec);
    }

    public WeatherLocationEditContext(DboWeatherLocation item)
        => this.Load(item);

    public override void Load(DboWeatherLocation record, bool notify = true)
    {
        this.BaseRecord = record with { };

        this.Uid = record.Uid;
        this.OwnerId = record.OwnerId;
        this.Location = record.Location;

        if (notify)
            this.NotifyFieldChanged(null);
    }


    public override DboWeatherLocation Record =>
        new DboWeatherLocation()
        {
            Uid = this.Uid,
            OwnerId = this.OwnerId,
            Location = this.Location
        };

    public override DboWeatherLocation AsNewRecord()
        => new DboWeatherLocation()
        {
            Uid = _newId,
            OwnerId = this.OwnerId,
            Location = this.Location
        };

    public override ValidationResult Validate(FieldReference? field = null)
    {
        var result = WeatherLocationValidator.Validate(this.Record, ValidationMessages, field);
        this.NotifyValidationStateUpdated(result.IsValid, field);
        return result;
    }
}
