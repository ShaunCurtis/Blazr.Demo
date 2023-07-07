/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components;

public class ModalOptions : IEnumerable<KeyValuePair<string, object>>
{
    /// <summary>
    /// List of options
    /// </summary>
    public static readonly string __Width = "Width";
    public static readonly string __ID = "ID";
    public static readonly string __ExitOnBackGroundClick = "ExitOnBackGroundClick";
    public static readonly string __BsSize = "BsSize";

    public Dictionary<string, object> ControlParameters { get; } = new Dictionary<string, object>();

    private Dictionary<string, object> OptionsList { get; } = new Dictionary<string, object>();

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        foreach (var item in OptionsList)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    public T? Get<T>(string key)
    {
        if (this.OptionsList.ContainsKey(key))
        {
            if (this.OptionsList[key] is T t) return t;
        }
        return default;
    }

    public bool TryGet<T>(string key, [NotNullWhen(true)] out T? value)
    {
        value = default;
        if (this.OptionsList.ContainsKey(key))
        {
            if (this.OptionsList[key] is T t)
            {
                value = t;
                return true;
            }
        }
        return false;
    }

    public bool Set(string key, object value)
    {
        if (this.OptionsList.ContainsKey(key))
        {
            this.OptionsList[key] = value;
            return false;
        }
        this.OptionsList.Add(key, value);
        return true;
    }
}

