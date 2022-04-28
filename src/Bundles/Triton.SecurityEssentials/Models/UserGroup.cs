namespace TheXDS.Triton.Models;

/// <summary>
/// Modelo que representa a un grupo de usuarios que comparten ciertas
/// facultades, propiedades y permisos de seguridad.
/// </summary>
public class UserGroup : SecurityObject
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="UserGroup"/>.
    /// </summary>
    public UserGroup()
        : this(null!, default, default)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="UserGroup"/>.
    /// </summary>
    /// <param name="displayName">Nombre a mostrar para esta entidad.</param>
    /// <param name="granted">Banderas que describen los permisos otorgados.</param>
    /// <param name="revoked">Banderas que describen los permisos denegados.</param>
    public UserGroup(string displayName, PermissionFlags granted, PermissionFlags revoked) : base(granted, revoked)
    {
        DisplayName = displayName;
    }

    /// <summary>
    /// Obtiene o establece el nombre a mostrar para esta entidad.
    /// </summary>
    public string DisplayName { get; set; }
}
