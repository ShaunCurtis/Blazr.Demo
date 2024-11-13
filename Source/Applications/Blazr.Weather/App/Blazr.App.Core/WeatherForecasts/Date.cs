/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct Date
{
    public DateOnly Value { get; init; }
    public bool Valid { get; init; }

    public Date() { }

    public Date(DateOnly date)
    {
        this.Value = date;
        if (date > DateOnly.MinValue)
            this.Valid = true;
    }

    public Date(DateTime date)
    {
        this.Value = DateOnly.FromDateTime(date);
        if (date > DateTime.MinValue)
            this.Valid = true;
    }

    public Date(DateTimeOffset date)
    {
        this.Value = DateOnly.FromDateTime(date.DateTime);
        if (date > DateTime.MinValue)
            this.Valid = true;
    }
}
