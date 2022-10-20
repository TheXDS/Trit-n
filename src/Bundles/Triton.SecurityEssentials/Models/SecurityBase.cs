using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models;

/// <summary>
/// Clase base para los modelos que contengan banderas de seguridad.
/// </summary>
public abstract class SecurityBase : Model<Guid>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SecurityBase"/>.
    /// </summary>
    protected SecurityBase() : this(default, default)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SecurityBase"/>.
    /// </summary>
    /// <param name="granted">Banderas que describen los permisos otorgados.</param>
    /// <param name="revoked">Banderas que describen los permisos denegados.</param>
    protected SecurityBase(PermissionFlags granted, PermissionFlags revoked)
    {
        Granted = granted;
        Revoked = revoked;
    }

    /// <summary>
    /// Obtiene o establece las banderas que describen los permisos otorgados
    /// al objeto de seguridad que contenga a esta entidad.
    /// </summary>
    public PermissionFlags Granted { get; set; }

    /// <summary>
    /// Obtiene o establece las banderas que describen los permisos otorgados
    /// al objeto de seguridad que contenga a esta entidad.
    /// </summary>
    public PermissionFlags Revoked { get; set; }
}
