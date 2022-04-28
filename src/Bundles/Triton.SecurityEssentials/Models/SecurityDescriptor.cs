namespace TheXDS.Triton.Models;

/// <summary>
/// Modelo que representa a un descriptor de seguridad que indica permisos
/// otorgados y/o denegados a una entidad de seguridad con respecto a un
/// determinado contexto.
/// </summary>
public class SecurityDescriptor : SecurityBase
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    ///  <see cref="SecurityDescriptor"/>.
    /// </summary>
    public SecurityDescriptor() : this(null!,default, default)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    ///  <see cref="SecurityDescriptor"/>.
    /// </summary>
    /// <param name="contextId">
    /// Valor que indica un Id del contexto al cual se aplicará este descriptor
    /// de seguridad.
    /// </param>
    /// <param name="granted">
    /// Banderas que describen los permisos otorgados.
    /// </param>
    /// <param name="revoked">
    /// Banderas que describen los permisos denegados.
    /// </param>
    public SecurityDescriptor(string contextId, PermissionFlags granted, PermissionFlags revoked) : base(granted, revoked)
    {
        ContextId = contextId;
    }

    /// <summary>
    /// Obtiene o establece el Id de contexto al cual se aplicarán las banderas
    /// de seguridad de esta entidad.
    /// </summary>
    public string ContextId { get; set; }
}
