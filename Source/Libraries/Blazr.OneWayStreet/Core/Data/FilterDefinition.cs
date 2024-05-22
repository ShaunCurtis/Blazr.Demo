/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core;

public record struct FilterDefinition(string FilterName,string FilterData)
{ 
    public bool TryFromJson<T>([NotNullWhen(true)] out T? value)
    {
        JsonSerializerOptions options = new() { IncludeFields = true };
        value = JsonSerializer.Deserialize<T>(this.FilterData, options);
        return value is not null;
    }

    public T? FromJson<T>()
        => JsonSerializer.Deserialize<T>(this.FilterData);

    public static FilterDefinition ToJson<T>(string name, T obj)
    {
        JsonSerializerOptions options = new() { IncludeFields = true };
        var json = JsonSerializer.Serialize<T>(obj, options);
        return new(name, json);
    }
}
