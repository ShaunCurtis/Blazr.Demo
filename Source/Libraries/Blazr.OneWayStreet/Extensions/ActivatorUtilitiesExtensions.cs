/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.OneWayStreet.Core.Extensions;

public static class ServiceUtilities
{
    public static bool TryGetComponentService<TService>(this IServiceProvider serviceProvider, [NotNullWhen(true)] out TService? service) where TService : class
    {
        service = serviceProvider.GetComponentService<TService>();
        return service != null;
    }

    public static TService? GetComponentService<TService>(this IServiceProvider serviceProvider) where TService : class
    {
        var serviceType = serviceProvider.GetService<TService>()?.GetType();

        if (serviceType is null)
            try
            {
                var service = ActivatorUtilities.CreateInstance<TService>(serviceProvider);
                return service;
            }
            catch
            {
                return null;
            }

        return ActivatorUtilities.CreateInstance(serviceProvider, serviceType) as TService;
    }
}
