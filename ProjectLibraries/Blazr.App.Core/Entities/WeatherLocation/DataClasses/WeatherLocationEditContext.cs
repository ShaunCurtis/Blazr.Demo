/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherLocationEditContext : RecordEditContextBase<DboWeatherLocation>
{
    private Guid _newId = Guid.NewGuid();

    private Guid _uid = Guid.Empty;
    public Guid Uid
    {
        get => _uid;
        set => SetIfChanged<Guid>(ref _uid, value, WeatherLocationConstants.Uid);
    }

    private Guid _ownerId = Guid.Empty;
    public Guid OwnerId
    {
        get => _ownerId;
        set => SetIfChanged<Guid>(ref _uid, value, WeatherLocationConstants.OwnerId);
    }

    private string _location = String.Empty;
    public string Location
    {
        get => _location;
        set => SetIfChanged<string>(ref _location, value, WeatherLocationConstants.Location);
    }

    public WeatherLocationEditContext(DboWeatherLocation record)
        : base(record)
        => this.Load(record);

    public override void Load(DboWeatherLocation record)
    {
        this.BaseRecord = record with { };   
        _uid = record.Uid;
        _ownerId = record.OwnerId;
        _location = record.Location;
    }
    public override void Reset()
        => this.Load(this.BaseRecord with { });

    public DboWeatherLocation AsNewRecord
        => CurrentRecord with { Uid=_newId };

    public override DboWeatherLocation CurrentRecord 
        => new DboWeatherLocation
        {
            Uid = _uid,
            OwnerId = _ownerId,
            Location = _location,
        };

    public override ValidationResult Validate(string? fieldname = null)
    {
        var result = WeatherLocationValidator.Validate(this.CurrentRecord, ValidationMessages, fieldname);
        this.NotifyValidationStateUpdated(result.IsValid, fieldname);
        return result;
    }
}
