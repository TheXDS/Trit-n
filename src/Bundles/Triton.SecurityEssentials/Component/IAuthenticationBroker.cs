using System.Security;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Component;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que actúe como 
/// un proveedor de autenticación para un servicio de Tritón.
/// </summary>
public interface IAuthenticationBroker : ISecurityActorProvider
{
    /// <summary>
    /// Obtiene un valor que indica si el proveedor de autenticación actual
    /// está elevado.
    /// </summary>
    bool Elevated { get; }

    /// <summary>
    /// Obtiene una referencia a la credencial utilizada en este proveedor de
    /// autenticación.
    /// </summary>
    SecurityObject? Credential { get; }

    /// <summary>
    /// Comprueba si la credencial especificada puede ser elevada para usarse
    /// en un servicio (suplantar a la credencial actualmente asociada).
    /// </summary>
    /// <param name="actor">Objeto de seguridad a comprobar.</param>
    /// <returns>
    /// <see langword="true"/> si la credencial especificada contiene el
    /// permiso  <see cref="PermissionFlags.Elevate"/> para el contexto de
    /// datos actual, <see langword="false"/> en caso contrario.
    /// </returns>
    bool CanElevate(SecurityObject? actor);

    /// <summary>
    /// Comprueba si la credencial actualmente asociada a un servicio puede
    /// ser elevada (suplantada por una con permisos distintos).
    /// </summary>
    /// <returns>
    /// <see langword="true"/> si la credencial activa contiene el permiso 
    /// <see cref="PermissionFlags.Elevate"/> para el contexto de datos actual,
    /// <see langword="false"/> en caso contrario.
    /// </returns>
    bool CanElevate()
    {
        var actor = GetActor();
        return actor is not null && CanElevate(actor);
    }

    /// <summary>
    /// Autentica a una entidad de seguridad como la que ejecuta operaciones
    /// utilizando este proveedor de autenticación.
    /// </summary>
    /// <param name="credential">Credencial a utilizar.</param>
    void Authenticate(SecurityObject? credential);

    /// <summary>
    /// Suplanta la credencial asociada a este proveedor de autenticación
    /// de manera temporal por una con permisos distintos.
    /// </summary>
    /// <param name="username">Nombre de usuario.</param>
    /// <param name="password">Contraseña.</param>
    /// <returns>
    /// <see cref="ServiceResult.Ok"/> si la elevación ha sido exitosa, o un
    /// <see cref="ServiceResult"/> cuyo motivo de falla es
    /// <see cref="FailureReason.Forbidden"/> en caso que la elevación haya 
    /// fracasado.
    /// </returns>
    Task<ServiceResult<Session?>> Elevate(string username, SecureString password);

    /// <summary>
    /// Revoca la elevación activa del servicio.
    /// </summary>
    void RevokeElevation();
}
