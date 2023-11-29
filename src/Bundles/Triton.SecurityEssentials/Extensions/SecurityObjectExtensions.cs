using TheXDS.Triton.Models;

namespace TheXDS.Triton.Extensions;

/// <summary>
/// Contiene métodos de extension para objetos de tipo
/// <see cref="SecurityObject"/>.
/// </summary>
public static class SecurityObjectExtensions
{
    /// <summary>
    /// Agrega al objeto de seguridad a un grupo de usuarios.
    /// </summary>
    /// <param name="obj">Objeto a agregar.</param>
    /// <param name="groups">
    /// Grupos a los cuales el objeto de seguridad pertenece.
    /// </param>
    public static void AddToGroup(this SecurityObject obj, params UserGroup[] groups)
    {
        foreach (var group in groups)
        {
            obj.Membership.Add(new() { Group = group, SecurityObject = obj });
        }
    }
}
