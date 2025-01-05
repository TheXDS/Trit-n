using System.Security;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Component;

/// <summary>
/// Defines the contract for types that provide authentication services for a
/// Triton service.
/// </summary>
public interface IAuthenticationBroker : ISecurityActorProvider
{
    /// <summary>
    /// Gets a value indicating whether the current authentication provider is
    /// elevated.
    /// </summary>
    bool IsElevated { get; }

    /// <summary>
    /// Gets the credential used for authentication.
    /// </summary>
    SecurityObject? Credential { get; }

    /// <summary>
    /// Checks if a credential can be elevated for use in a service.
    /// </summary>
    /// <param name="credential">The credential to check.</param>
    /// <returns>
    /// <see langword="true"/> if the credential can be elevated,
    /// <see langword="false"/> otherwise.
    /// </returns>
    bool CanElevate(SecurityObject? credential);

    /// <summary>
    /// Checks if the currently associated credential can be elevated.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the credential can be elevated,
    /// <see langword="false"/> otherwise.
    /// </returns>
    bool CanElevate()
    {
        var actor = GetCurrentActor();
        return actor is not null && CanElevate(actor);
    }

    /// <summary>
    /// Authenticates a security entity using the provided credential.
    /// </summary>
    /// <param name="credential">
    /// The credential to use for authentication.
    /// </param>
    void Authenticate(SecurityObject? credential);

    /// <summary>
    /// Temporarily elevates the credential associated with this authentication
    /// provider.
    /// </summary>
    /// <param name="username">The username to use for elevation.</param>
    /// <param name="password">The password to use for elevation.</param>
    /// <returns>
    /// A service result indicating whether elevation was successful.
    /// </returns>
    Task<ServiceResult<Session?>> ElevateAsync(string username, SecureString password);

    /// <summary>
    /// Revokes the active elevation of the service.
    /// </summary>
    void RevokeElevation();
}
