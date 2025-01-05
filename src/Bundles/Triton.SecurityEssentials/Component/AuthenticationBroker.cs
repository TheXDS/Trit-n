using System.Security;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Component;

/// <summary>
/// Implements an authentication provider that runs at the beginning of a
/// transaction for any Triton service.
/// </summary>
public class AuthenticationBroker : IAuthenticationBroker
{
    private readonly IUserService _userService;
    private SecurityObject? _credential;
    private SecurityObject? _elevation;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationBroker"/>
    /// class.
    /// </summary>
    /// <param name="middlewareConfigurator">
    /// Instance of middleware configurator where this authentication provider
    /// will be registered.
    /// </param>
    /// <param name="userService">
    /// Instance of user service to use for authenticating and verifying access
    /// permissions of a <see cref="SecurityObject"/>.
    /// </param>
    public AuthenticationBroker(IMiddlewareConfigurator middlewareConfigurator, IUserService userService)
    {
        middlewareConfigurator.Attach(new DataLayerSecurityMiddleware(this, userService));
        _userService = userService;
    }

    /// <inheritdoc/>
    SecurityObject? IAuthenticationBroker.Credential => _credential;

    /// <inheritdoc/>
    bool IAuthenticationBroker.IsElevated => _elevation != null;

    void IAuthenticationBroker.Authenticate(SecurityObject? credential)
    {
        _credential = credential;
    }

    /// <inheritdoc/>
    bool IAuthenticationBroker.CanElevate(SecurityObject? actor) => actor is not null && (_userService.CheckAccess(actor, GetType().Name, PermissionFlags.Elevate).Result ?? false);

    /// <inheritdoc/>
    async Task<ServiceResult<Session?>> IAuthenticationBroker.ElevateAsync(string username, SecureString password)
    {
        if (((IAuthenticationBroker)this).CanElevate())
        {
            var cred = await _userService.Authenticate(username, password);
            if (cred.Success)
            {
                _elevation = cred.Result!.Credential;
            }
            return cred;
        }
        else return FailureReason.Forbidden;
    }

    /// <summary>
    /// Retrieves the security principal that executes operations using this
    /// service.
    /// </summary>
    /// <returns>
    /// The security actor responsible for executing operations on this service.
    /// </returns>
    SecurityObject? ISecurityActorProvider.GetActor() => _elevation ?? _credential;

    /// <inheritdoc/>
    void IAuthenticationBroker.RevokeElevation() => _elevation = null;
}
