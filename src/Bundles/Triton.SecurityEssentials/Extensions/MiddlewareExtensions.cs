using TheXDS.Triton.Component;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Extensions;

/// <summary>
/// Provides extensions for configuring middlewares.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Registers an authentication provider with the service's middleware
    /// pipeline.
    /// </summary>
    /// <param name="configurator">
    /// The middleware configurator instance where the authentication provider
    /// will be registered.
    /// </param>
    /// <param name="userService">
    /// The user service instance used for credential authentication and
    /// permission checks.
    /// </param>
    /// <param name="broker">
    /// Output parameter. The registered authentication provider instance.
    /// </param>
    /// <returns>
    /// The same middleware configurator instance, enabling fluent syntax
    /// usage.
    /// </returns>
    public static IMiddlewareConfigurator AddAuthentication(
        this IMiddlewareConfigurator configurator,
        IUserService userService, out IAuthenticationBroker broker)
    {
        broker = new AuthenticationBroker(configurator, userService);
        return configurator;
    }
}