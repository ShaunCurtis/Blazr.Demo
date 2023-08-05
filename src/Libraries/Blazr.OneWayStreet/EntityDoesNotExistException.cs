namespace Blazr.OneWayStreet;

/// <summary>
/// Exception raised when the state does not exist in the store
/// </summary>
public class StateDoesNotExistsException : Exception
{
    public StateDoesNotExistsException(string message) : base(message)
    { }
}
