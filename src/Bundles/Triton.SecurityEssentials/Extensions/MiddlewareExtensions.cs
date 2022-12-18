using TheXDS.Triton.Component;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Extensions;

/// <summary>
/// Incluye extensiones para la interfaz <see cref="IMiddlewareConfigurator"/>.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Agrega soporte para proveedor de autenticación a la colección de
    /// Middlewares del servicio.
    /// </summary>
    /// <param name="configurator">
    /// Instancia de Middlewares en la cual registrar el proveedor de
    /// autenticación.
    /// </param>
    /// <param name="userService">Instancia del servicio de usuarios a utilizar
    /// al autenticar credenciales y comprobar permisos.
    /// </param>
    /// <param name="broker">
    /// Parámetro de salida. Instancia del proveedor de autenticación
    /// registrado para el servicio a configurar.
    /// </param>
    /// <returns>
    /// La misma instancia que <paramref name="configurator"/>, permitiendo el
    /// uso de sintaxis Fluent.
    /// </returns>
    public static IMiddlewareConfigurator AddAuthentication(
        this IMiddlewareConfigurator configurator,
        IUserService userService, out IAuthenticationBroker broker)
    {
        broker = new AuthenticationBroker(configurator, userService);
        return configurator;
    }
}
