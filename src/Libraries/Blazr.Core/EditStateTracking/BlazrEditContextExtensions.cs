/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

using Microsoft.AspNetCore.Components.Forms;

public static class BlazrEditContextExtensions
{
    public const string EditStateStoreName = "EditStateStore";

    public static bool GetEditState(this EditContext editContext)
    {
        BlazrEditStateStore? store = null;
        if (editContext.Properties.TryGetValue(EditStateStoreName, out object? value))
            store = value as BlazrEditStateStore;

        return store?.IsDirty() ?? false;
    }

    public static void ResetEditState(this EditContext editContext)
    {
        if (editContext.Properties.TryGetValue(EditStateStoreName, out object? value))
        {
            var store = value as BlazrEditStateStore;
            store?.Reset();
            var recordEditContext = editContext.Model as IBlazrEditContext;
            recordEditContext?.Reset();
        }
    }

    public static void SetEditStateAsSaved(this EditContext editContext)
    {
        if (editContext.Properties.TryGetValue(EditStateStoreName, out object? value))
        {
            var store = value as BlazrEditStateStore;
            store?.Reset();
            var recordEditContext = editContext.Model as IBlazrEditContext;
            recordEditContext?.SetAsSaved();
        }
    }

    public static BlazrEditStateStore? GetStateStore(this EditContext editContext)
    {
        BlazrEditStateStore? store = null;
        if (editContext.Properties.TryGetValue(EditStateStoreName, out object? value))
            store = value as BlazrEditStateStore;

        return store;
    }

    public static bool TryGetStateStore(this EditContext editContext, [NotNullWhen(true)] out BlazrEditStateStore? store)
    {
        store = null;
        if (editContext.Properties.TryGetValue(EditStateStoreName, out object? value))
            store = value as BlazrEditStateStore;

        return store is not null;
    }

    public static bool IsFieldValid(this EditContext editContext, FieldIdentifier? fieldIdentifier)
    {
        var messages = editContext.GetValidationMessages(fieldIdentifier ?? new());
        return messages is null || messages.Count() == 0;
    }

    public static bool IsFieldValid(this EditContext editContext, Expression<Func<string>>? expression)
    {
        if (TryGetFieldIdentifier(expression, out var fieldIdentifier))
            return editContext.GetValidationMessages(fieldIdentifier ?? new()) is null;

        return false;
    }
    private static bool TryGetFieldIdentifier(Expression<Func<string>>? expression, [NotNullWhen(true)] out FieldIdentifier? fi)
    {
        fi = null;
        if (expression is null)
            return false;

        fi = FieldIdentifier.Create(expression);
        return fi is not null;
    }

}
