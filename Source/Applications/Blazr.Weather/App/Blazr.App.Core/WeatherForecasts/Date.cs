/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct Date
{
    public DateOnly Value { get; init; }
    public bool IsValid { get; init; }

    public Date() { }

    public Date(DateOnly date)
    {
        this.Value = date;
        if (date > DateOnly.MinValue)
            this.IsValid = true;
    }

    public Date(DateTime date)
    {
        this.Value = DateOnly.FromDateTime(date);
        if (date > DateTime.MinValue)
            this.IsValid = true;
    }

    public Date(DateTimeOffset date)
    {
        this.Value = DateOnly.FromDateTime(date.DateTime);
        if (date > DateTime.MinValue)
            this.IsValid = true;
    }

    public override string ToString()
    {
        return this.IsValid ? this.Value.ToString("dd-MMM-yy")  : "Not Valid";
    }
}
