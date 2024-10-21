namespace TheXDS.Triton.Models;

/// <summary>
/// Modelo que representa a un grupo de usuarios que comparten ciertas
/// facultades, propiedades y permisos de seguridad.
/// </summary>
/// <param name="displayName">Nombre a mostrar para esta entidad.</param>
/// <param name="granted">
/// Banderas que describen los permisos otorgados.
/// </param>
/// <param name="revoked">
/// Banderas que describen los permisos denegados.
/// </param>
public class UserGroup(string displayName, PermissionFlags granted, PermissionFlags revoked) : SecurityObject(granted, revoked)
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="UserGroup"/>, estableciendo todas sus propiedades a sus
    /// valores predeterminados.
    /// </summary>
    public UserGroup() : this(null!, default, default)
    {
    }

    /// <summary>
    /// Obtiene o establece el nombre a mostrar para esta entidad.
    /// </summary>
    public string DisplayName { get; set; } = displayName;
}
