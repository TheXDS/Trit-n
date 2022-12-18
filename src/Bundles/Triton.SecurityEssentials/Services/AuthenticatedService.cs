using TheXDS.Triton.Component;

namespace TheXDS.Triton.Services;

/// <summary>
/// Clase base para un servicio de tritón con soporte para autenticación.
/// </summary>
public abstract class AuthenticatedService : TritonService, IAuthenticable
{
    /// <inheritdoc/>
    public IAuthenticationBroker AuthenticationBroker { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="AuthenticatedService"/>.
    /// </summary>
    /// <param name="userService">
    /// Servicio de usuario a utilizar internamente por el proveedor de
    /// autenticación.
    /// </param>
    public AuthenticatedService(IUserService userService)
    {
        AuthenticationBroker = new AuthenticationBroker(Configuration, userService);
    }
}
