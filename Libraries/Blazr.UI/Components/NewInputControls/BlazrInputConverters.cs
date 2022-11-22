/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public static class BlazrInputConverters
{
    public static string? GetValueAsString<TValue>(TValue? initialValue, string? type) =>
        initialValue switch
        {
            string @value => value,
            int @value => BindConverter.FormatValue(value),
            long @value => BindConverter.FormatValue(value),
            short @value => BindConverter.FormatValue(value),
            float @value => BindConverter.FormatValue(value),
            double @value => BindConverter.FormatValue(value),
            decimal @value => BindConverter.FormatValue(value),
            TimeOnly @value => BindConverter.FormatValue(value, format: "HH:mm:ss"),
            DateOnly @value => BindConverter.FormatValue(value, format: "yyyy-MM-dd"),
            DateTime @value => GetDateString(value, type),
            DateTimeOffset @value => GetDateString(value, type),
            _ => initialValue?.ToString() ?? throw new InvalidOperationException($"Unsupported type {initialValue?.GetType()}")
        };

    public static string GetDateString(DateTime value, string? type)
        => type?.ToLower() switch
        {
            "date" => BindConverter.FormatValue(value, format: "yyyy-MM-dd"),
            "time" => BindConverter.FormatValue(value, format: "HH:mm:ss"),
            _ => BindConverter.FormatValue(value, format: "yyyy-MM-ddTHH:mm:ss")
        };

    public static string GetDateString(DateTimeOffset value, string? type)
        => type?.ToLower() switch
        {
            "date" => BindConverter.FormatValue(value, format: "yyyy-MM-dd"),
            "time" => BindConverter.FormatValue(value, format: "HH:mm:ss"),
            _ => BindConverter.FormatValue(value, format: "yyyy-MM-ddTHH:mm:ss")
        };
}
