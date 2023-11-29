using System.Security;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Component;

/// <summary>
/// Implementa un proveedor de autenticación que se ejecuta en el prólogo de
/// una transacción para cualquier servicio de Tritón.
/// </summary>
public class AuthenticationBroker : IAuthenticationBroker
{
    private readonly IUserService _userService;
    private SecurityObject? _credential;
    private SecurityObject? _elevation;

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="AuthenticationBroker"/>.
    /// </summary>
    /// <param name="middlewareConfigurator">
    /// Instancia de configuración de Middleware en la cual registrar este
    /// proveedor de autenticación.
    /// </param>
    /// <param name="userService">
    /// Instancia de servicio de usuario a utilizar para autenticar y verificar
    /// los permisos de acceso de un <see cref="SecurityObject"/>.
    /// </param>
    public AuthenticationBroker(IMiddlewareConfigurator middlewareConfigurator, IUserService userService)
    {
        middlewareConfigurator.Attach(new DataLayerSecurityMiddleware(this, userService));
        _userService = userService;
    }

    /// <inheritdoc/>
    SecurityObject? IAuthenticationBroker.Credential => _credential;

    /// <inheritdoc/>
    bool IAuthenticationBroker.Elevated => _elevation != null;

    void IAuthenticationBroker.Authenticate(SecurityObject? credential)
    {
        _credential = credential;
    }

    /// <inheritdoc/>
    bool IAuthenticationBroker.CanElevate(SecurityObject? actor) => actor is not null && (_userService.CheckAccess(actor, GetType().Name, PermissionFlags.Elevate).Result ?? false);

    /// <inheritdoc/>
    async Task<ServiceResult<Session?>> IAuthenticationBroker.Elevate(string username, SecureString password)
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
        else return ServiceResult.FailWith<ServiceResult<Session?>>(FailureReason.Forbidden);
    }

    /// <summary>
    /// Obtiene al objeto de seguridad que ejecuta las operaciones de este
    /// servicio.
    /// </summary>
    /// <returns>El actor que ejecuta operaciones en este servicio.</returns>
    SecurityObject? ISecurityActorProvider.GetActor() => _elevation ?? _credential;

    /// <inheritdoc/>
    void IAuthenticationBroker.RevokeElevation() => _elevation = null;
}
