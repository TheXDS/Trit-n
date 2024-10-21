namespace TheXDS.Triton.Models;

/// <summary>
/// Clase base para las entidades que soportan descriptores de seguridad y
///  pertenencia a grupos.
/// </summary>
/// <param name="granted">Banderas que describen los permisos otorgados.</param>
/// <param name="revoked">Banderas que describen los permisos denegados.</param>
public abstract class SecurityObject(PermissionFlags granted, PermissionFlags revoked) : SecurityBase(granted, revoked)
{
    /// <summary>
    /// Obtiene o establece una coleccion que define las pertenencias a grupos
    /// de usuario para la entidad actual.
    /// </summary>
    public virtual ICollection<UserGroupMembership> Membership { get; set; } = [];

    /// <summary>
    /// Obtiene o establece una colección que contiene a los descriptores de
    /// seguridad disponibles para esta entidad.
    /// </summary>
    public virtual ICollection<SecurityDescriptor> Descriptors { get; set; } = [];

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SecurityObject"/>.
    /// </summary>
    protected SecurityObject()
        : this(default, default)
    {
    }
}
