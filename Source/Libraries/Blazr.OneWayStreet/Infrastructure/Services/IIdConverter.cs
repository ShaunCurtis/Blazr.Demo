/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Infrastructure;

public interface IIdConverter
{
    public object Convert(object value);

    public bool TryConvert(object inValue, out object? outValue );
}
