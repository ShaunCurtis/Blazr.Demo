/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components;

public sealed class CSSBuilder
{
    private Queue<string> _cssQueue = new Queue<string>();

    public static CSSBuilder Class(string? cssFragment = null)
        => new CSSBuilder(cssFragment);

    public CSSBuilder() { }

    public CSSBuilder(string? cssFragment)
        => AddClass(cssFragment ?? String.Empty);

    public CSSBuilder AddClass(string? cssFragment)
    {
        if (!string.IsNullOrWhiteSpace(cssFragment))
            _cssQueue.Enqueue(cssFragment);
        return this;
    }

    public CSSBuilder AddClass(IEnumerable<string> cssFragments)
    {
        cssFragments.ToList().ForEach(item => _cssQueue.Enqueue(item));
        return this;
    }

    public CSSBuilder AddClass(bool WhenTrue, string cssFragment)
        => WhenTrue ? this.AddClass(cssFragment) : this;

    public CSSBuilder AddClass(bool WhenTrue, string? trueCssFragment, string? falseCssFragment)
        => WhenTrue ? this.AddClass(trueCssFragment) : this.AddClass(falseCssFragment);

    public CSSBuilder AddClassFromAttributes(IReadOnlyDictionary<string, object> additionalAttributes)
    {
        if (additionalAttributes != null && additionalAttributes.TryGetValue("class", out var val))
            _cssQueue.Enqueue(val.ToString() ?? string.Empty);
        return this;
    }

    public CSSBuilder AddClassFromAttributes(IDictionary<string, object> additionalAttributes)
    {
        if (additionalAttributes != null && additionalAttributes.TryGetValue("class", out var val))
            _cssQueue.Enqueue(val.ToString() ?? string.Empty);
        return this;
    }

    public string Build(string? CssFragment = null)
    {
        if (!string.IsNullOrWhiteSpace(CssFragment)) _cssQueue.Enqueue(CssFragment);
        if (_cssQueue.Count == 0)
            return string.Empty;
        var sb = new StringBuilder();
        foreach (var str in _cssQueue)
        {
            if (!string.IsNullOrWhiteSpace(str)) sb.Append($" {str}");
        }
        return sb.ToString().Trim();
    }
}

