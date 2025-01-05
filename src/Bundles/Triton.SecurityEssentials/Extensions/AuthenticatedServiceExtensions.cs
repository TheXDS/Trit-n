using System.Security;
using TheXDS.Triton.Component;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Extensions;

/// <summary>
/// Provides extensions for authenticated Triton services.
/// </summary>
public static class AuthenticatedServiceExtensions
{
    /// <summary>
    /// Executes an action on the service with elevated privileges.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of service to elevate, which must implement
    /// <see cref="AuthenticatedService"/>.
    /// </typeparam>
    /// <param name="service">The instance of the service to elevate.</param>
    /// <param name="username">The username to use for elevation.</param>
    /// <param name="password">The password to use for elevation.</param>
    /// <param name="elevatedCallback">
    /// The action to execute in an elevated context.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult"/> indicating the outcome of the elevation
    /// attempt.
    /// If successful, returns <see cref="ServiceResult.Ok"/>; otherwise,
    /// returns a result with a failure reason.
    /// </returns>
    /// <remarks>
    /// Elevation is automatically revoked after executing the elevated action.
    /// For persistent elevation, use the
    /// <see cref="IAuthenticationBroker.ElevateAsync(string, SecureString)"/>
    /// method on the <see cref="AuthenticatedService.AuthenticationBroker"/>
    /// property.
    /// </remarks>
    public static async Task<ServiceResult> Sudo<TService>(
        this TService service,
        string username,
        SecureString password,
        Func<TService, Task> elevatedCallback) where TService : AuthenticatedService
    {
        var elevationResult = await service.AuthenticationBroker.ElevateAsync(username, password);
        if (elevationResult.Success)
        {
            try
            {
                await elevatedCallback(service);
            }
            finally
            {
                service.AuthenticationBroker.RevokeElevation();
            }
        }
        return elevationResult;
    }
}
