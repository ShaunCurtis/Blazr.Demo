using System;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;
public static class RenderTreeBuilderExtensions
{
    public static void AddAttributeIfNotNull(this RenderTreeBuilder builder, int sequenceNo, string attribute , object? value)
    {
        if (value is not null)
            builder.AddAttribute(sequenceNo, attribute, value);
    }

    public static void AddAttributeIfNotEmpty(this RenderTreeBuilder builder, int sequenceNo, string attribute, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddAttribute(sequenceNo, attribute, value);
    }

    public static void AddAttributeIfTrue(this RenderTreeBuilder builder, bool check, int sequenceNo, string attribute, object? value = null)
    {
        if (check && value is null)
            builder.AddAttribute(sequenceNo, attribute);

        else if (check)
            builder.AddAttribute(sequenceNo, attribute, value);
    }

    public static void AddContentIfTrue(this RenderTreeBuilder builder, bool check, int sequenceNo, RenderFragment? value)
    {
        if (check && value is not null)
            builder.AddContent(sequenceNo, value);
    }

    public static void AddContentIfTrue(this RenderTreeBuilder builder, bool check, int sequenceNo, MarkupString? value)
    {
        if (check && value is not null)
            builder.AddContent(sequenceNo, value);
    }
    public static void AddContentIfTrue(this RenderTreeBuilder builder, bool check, int sequenceNo, string? value)
    {
        if (check && value is not null)
            builder.AddContent(sequenceNo, value);
    }

    public static void AddContentIfNotNull(this RenderTreeBuilder builder, int sequenceNo, RenderFragment? value)
    {
        if (value is not null)
            builder.AddContent(sequenceNo, value);
    }

    public static void AddContentIfNotNull(this RenderTreeBuilder builder, int sequenceNo, MarkupString? value)
    {
        if (value is not null)
            builder.AddContent(sequenceNo, value);
    }

    public static void AddContentIfNotNull(this RenderTreeBuilder builder, int sequenceNo, string? value)
    {
        if (value is not null)
            builder.AddContent(sequenceNo, value);
    }

    public static void AddContentIfNotEmpty(this RenderTreeBuilder builder, int sequenceNo, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.AddContent(sequenceNo, value);
    }
}
