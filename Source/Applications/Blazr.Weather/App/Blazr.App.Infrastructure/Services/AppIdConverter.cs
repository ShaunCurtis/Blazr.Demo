/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class AppIdConverter : IIdConverter
{
    public object Convert(object value)
    {
        if (this.TryConvert(value, out object? outValue))
            return outValue;

        return value;
    }

    public bool TryConvert(object inValue, [NotNullWhen(true)] out object? outValue)
    {
        if (inValue is IRecordId id)
        {
            outValue = id.GetValueObject();
            return true;
        }

        outValue = null;
        return false;
    }
}
