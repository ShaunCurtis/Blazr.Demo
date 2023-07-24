/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class AppStateCodes
{
    public const int Record = 1;
    public const int New = 0;
    public const int Delete = int.MinValue;

    public static bool IsUpdate(int value) => value > 0;
    public static bool IsNew(int value) => value == 0;
    public static bool IsDeleted(int value) => value == Delete;
}
