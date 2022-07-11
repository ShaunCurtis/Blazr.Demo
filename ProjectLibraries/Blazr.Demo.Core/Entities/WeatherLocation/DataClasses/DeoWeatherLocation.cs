/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class DeoWeatherLocation
    : IEditRecord<DboWeatherLocation>, IValidation
{
    private DboWeatherLocation _baseRecord = new DboWeatherLocation();
    private Guid _newId = Guid.NewGuid();

    public Guid Id { get; set; } = GuidExtensions.Null;

    public string Location { get; set; } = string.Empty;

    public bool IsNull => Id == GuidExtensions.Null;

    public bool IsNew => Id == Guid.Empty;

    public bool IsDirty => _baseRecord != this.Record;

    public DboWeatherLocation CleanRecord => _baseRecord;

    public DeoWeatherLocation()
    {
        var rec = new DboWeatherLocation();
        this.Load(rec);
    }

    public DeoWeatherLocation(DboWeatherLocation item)
        => this.Load(item);

    public void Reset()
    => this.Load(_baseRecord with { });

    public void Load(DboWeatherLocation record)
    {
        _baseRecord = record with { };

        this.Id = record.WeatherLocationId;
        this.Location = record.Location;
    }

    public DboWeatherLocation Record =>
        new DboWeatherLocation()
        {
            WeatherLocationId = this.Id,
            Location = this.Location
        };

    public DboWeatherLocation AsNewRecord =>
        new DboWeatherLocation()
        {
            WeatherLocationId = _newId,
            Location = this.Location
        };

    public bool Validate(ValidationMessageStore? validationMessageStore, string? fieldname, object? model = null)
    {
        model = model ?? this;
        bool trip = false;

        this.Location.Validation("Location", model, validationMessageStore)
            .LongerThan(2, "The location miust be at least 2 characters")
            .Validate(ref trip, fieldname);

        return !trip;
    }
}
