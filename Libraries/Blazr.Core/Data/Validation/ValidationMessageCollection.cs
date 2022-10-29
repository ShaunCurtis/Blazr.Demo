/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core.Validation;

public record ValidationMessage(string Field, string Message) { }

public class ValidationMessageCollection : IEnumerable<ValidationMessage>
{
    private readonly List<ValidationMessage> _messages = new List<ValidationMessage>();

    public void Add(ValidationMessage message)
        => _messages.Add(message);

    public void Add( string field, string message)
        => _messages.Add(new ValidationMessage(field, message));

    public void Add(string field, IEnumerable<string> messages)
    {
        foreach (var message in messages)
            _messages.Add(new ValidationMessage(field, message));
    }

    public void ClearMessages(string field)
    {
        var messagesToDelete = _messages.Where(item => item.Field.Equals(field)).ToList();
        if (messagesToDelete is not null)
            foreach (var message in messagesToDelete)
                _messages.Remove(message);
    }

    public void ClearAllMessages()
        => _messages.Clear();

    public IEnumerable<string> GetMessages(string? field = null)
        => field is null
        ? _messages.Select(item => item.Message).AsEnumerable()
        : _messages.Where(item => item.Field.Equals(field)).Select(item => item.Message).AsEnumerable() ?? Enumerable.Empty<string>();

    public bool HasMessages(string? field = null)
        => field is null
        ? _messages.Any()
        : _messages.Any(item => item.Field.Equals(field));

    public IEnumerator<ValidationMessage> GetEnumerator()
        => _messages.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
