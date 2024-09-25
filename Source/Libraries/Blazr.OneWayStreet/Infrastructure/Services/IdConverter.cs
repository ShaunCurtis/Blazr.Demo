/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public class IdConverter : IIdConverter
{
    public object Convert(object value)
    {
        if (this.TryConvert(value,out object? outValue))
            return outValue;

        return value;
    }

    public bool TryConvert(object inValue, [NotNullWhen(true)] out object? outValue)
    {
        if (long.TryParse(inValue.ToString(), out long longValue))
        {
            outValue =    longValue;
            return true;
        }

        if (Guid.TryParse(inValue.ToString(), out Guid guidValue))
        {
            outValue = guidValue;
            return true;
        }

        outValue = null;
        return false;
    }
}
