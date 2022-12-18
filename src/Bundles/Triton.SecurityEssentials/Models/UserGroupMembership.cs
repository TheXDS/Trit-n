using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models;

/// <summary>
/// modelo que representa la membresía de un usuario a un grupo de usuarios.
/// </summary>
public class UserGroupMembership : Model<Guid>
{
    /// <summary>
    /// Obtiene o estabelce el grupo del cual el usuario es miembro.
    /// </summary>
    public UserGroup Group { get; set; } = null!;

    /// <summary>
    /// Obtiene o establece el objeto de seguridad que es miembro de un grupo.
    /// </summary>
    public SecurityObject SecurityObject { get; set; } = null!;
}