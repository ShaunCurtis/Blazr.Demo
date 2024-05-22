/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public class DataPipelineException : Exception
{
    public DataPipelineException() : base() { }
    public DataPipelineException(string message) : base(message) { }
    public DataPipelineException(string messge, Exception innnerException) : base(messge, innnerException) { }
}
