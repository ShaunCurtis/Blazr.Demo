/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.EditStateTracker;

public class BlazrEditStateStore
{
    private object _model = new();

    private List<EditStateProperty> _properties = new();
    private EditContext _editContext;

    public event EventHandler<FieldChangedEventArgs>? StoreUpdated;

    public BlazrEditStateStore(EditContext context)
    {
        _editContext = context;
        _model = context.Model;

        var props = _model.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(TrackStateAttribute)));

        foreach (var prop in props)
        {
            _properties.Add(new(prop.Name, prop.GetValue(_model)));
        }
    }

    public void Update(FieldChangedEventArgs e)
    {
        var property = _properties.FirstOrDefault(item => item.Name.Equals(e.FieldIdentifier.FieldName));

        if (property != null)
        {
            var propInfo = e.FieldIdentifier.Model.GetType().GetProperty(e.FieldIdentifier.FieldName);
            if (propInfo != null)
            {
                var value = propInfo.GetValue(e.FieldIdentifier.Model);
                property.Set(value);

                // If the value is clean clear out the modified setting in the Edit Context
                if (!IsDirty(e.FieldIdentifier.FieldName))
                    _editContext.MarkAsUnmodified(e.FieldIdentifier);

                this.StoreUpdated?.Invoke(this, e);
            }
        }
    }

    public void Reset()
    {
        foreach (var prop in _properties)
        {
            prop.Reset();
            _editContext.MarkAsUnmodified(new(_editContext, prop.Name));
        }
    }

    public bool IsDirty(string fieldName)
        => _properties.FirstOrDefault(item => item.Name.Equals(fieldName))?.IsDirty ?? false;

    public bool IsDirty()
        => _properties.Any(item => item.IsDirty);
}
