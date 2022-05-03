namespace TheXDS.Triton.Models;
using System;
using TheXDS.Triton.Models.Base;

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
    /// Obtiene o establece el usuario que es miembro de un grupo.
    /// </summary>
    public LoginCredential Credential { get; set; } = null!;
}