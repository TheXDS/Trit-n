using TheXDS.Triton.Models;

namespace TheXDS.Triton.Component;

/// <summary>
/// Define una serie de miembros a implementar por una clase que permita
/// obtener información acerca del actor que ejecuta operaciones en un
/// contexto de seguridad.
/// </summary>
public interface ISecurityActorProvider
{
    /// <summary>
    /// Obtiene al actor que ejecuta operaciones de seguridad en la
    /// aplicación actual.
    /// </summary>
    /// <returns>
    /// Una instancia de la clase <see cref="SecurityObject"/> que
    /// corresponde al actor que ejecuta operaciones de seguridad en la
    /// aplicación actual.
    /// </returns>
    SecurityObject? GetActor();
}
