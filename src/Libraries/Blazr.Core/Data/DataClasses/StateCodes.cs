/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class StateCodes
{
    public static StateCode Deleted = new(int.MinValue, "Deleted");
    public static StateCode New = new(0, "New");
    public static StateCode Existing = new(1, "Existing");
    public static StateCode Null = new(int.MaxValue, "Null");

    public static List<StateCode> StateCodeList = new()
    {
        Deleted,
        Existing,
        New,
        Null
    };

    public static StateCode GetStateCode(int code)
        => StateCodeList.FirstOrDefault(item => item.Value == code);
}
