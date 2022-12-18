using System.Security;
using TheXDS.Triton.Component;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Extensions;

/// <summary>
/// Contiene extensiones para los servicios de tritón que soportan
/// autenticación.
/// </summary>
public static class AuthenticatedServiceExtensions
{
    /// <summary>
    /// Eejcuta una acción en el servicio de forma elevada.
    /// </summary>
    /// <typeparam name="TService">
    /// Tipo de servicio a elevar. Debe heredar 
    /// <see cref="AuthenticatedService"/>.
    /// </typeparam>
    /// <param name="service">Instancia de servicio a elevar.</param>
    /// <param name="username">Nombre de usuario.</param>
    /// <param name="password">Contraseña.</param>
    /// <param name="elevatedCallback">
    /// Delegado a ejecutar en un contexto elevado.
    /// </param>
    /// <returns>
    /// <see cref="ServiceResult.Ok"/> si la elevación ha sido exitosa, o un
    /// <see cref="ServiceResult"/> cuyo motivo de falla es
    /// <see cref="FailureReason.Forbidden"/> en caso que la elevación haya 
    /// fracasado.
    /// </returns>
    /// <remarks>
    /// La elevación del servicio finalizará inmediatamente después de ejecutar
    /// la acción elevada solicitada. Para una elevación persistente, deberá
    /// elevar el servicio manualmente utilizando el método de instancia
    /// <see cref="IAuthenticationBroker.Elevate(string, SecureString)"/> de la
    /// propiedad <see cref="AuthenticatedService.AuthenticationBroker"/>.
    /// </remarks>
    public static async Task<ServiceResult> Sudo<TService>(
        this TService service,
        string username,
        SecureString password,
        Func<TService, Task> elevatedCallback) where TService : AuthenticatedService
    {
        var elevationResult = await service.AuthenticationBroker.Elevate(username, password);
        if (elevationResult.Success)
        { 
            await elevatedCallback(service);
            service.AuthenticationBroker.RevokeElevation();
        }
        return elevationResult;
    }
}
