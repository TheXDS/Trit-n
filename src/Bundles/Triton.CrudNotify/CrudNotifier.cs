using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.CrudNotify;

/// <summary>
/// Middleware that allows sending CRUD notifications over the network to other
/// clients connected through a custom TCP protocol.
/// </summary>
public static class CrudNotifier
{
    private static readonly List<ICrudNotifier> _notifiers = [];

    /// <summary>
    /// Adds a new instance of a CRUD notification service to all data
    /// transactions.
    /// </summary>
    /// <typeparam name="T">
    /// The type of notification service to instantiate.
    /// </typeparam>
    /// <param name="config">
    /// Transaction configuration object to configure.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>.
    /// </returns>
    public static IMiddlewareConfigurator AddNotifyService<T>(this IMiddlewareConfigurator config) where T : ICrudNotifier, new()
    {
        return config.AddNotifyService(new T());
    }

    /// <summary>
    /// Adds a CRUD notification service to all data transactions.
    /// </summary>
    /// <typeparam name="T">
    /// The type of notification service to add.
    /// </typeparam>
    /// <param name="config">
    /// Transaction configuration object to configure.
    /// </param>
    /// <param name="crudNotifier">
    /// Instance of a CRUD notification service to add to the data
    /// transactions.
    /// </param>
    /// <returns>
    /// The same instance as <paramref name="config"/>.
    /// </returns>
    public static IMiddlewareConfigurator AddNotifyService<T>(this IMiddlewareConfigurator config, T crudNotifier) where T : ICrudNotifier
    {
        return config.AddLateEpilogue(crudNotifier.PushInto(_notifiers).NotifyPeers);
    }
}