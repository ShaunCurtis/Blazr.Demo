/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public class NewRecordProvider : INewRecordProvider
{
    private readonly IServiceProvider _serviceProvider;

    public NewRecordProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TRecord NewRecord<TRecord>()
        where TRecord : new()
    {
        var service = _serviceProvider.GetService<INewRecordProvider<TRecord>>();

        if (service is not null)
            return service.NewRecord();

        // Default is return a newed up object instance
        return new TRecord();
    }
}
