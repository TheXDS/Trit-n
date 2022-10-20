namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que exponga
/// información sobre un actor que ejecuta una operación.
/// </summary>
public interface IActorProvider
{
    /// <summary>
    /// Obtiene el nombre descriptivo del actor que ejecutó la acción.
    /// </summary>
    /// <returns>
    /// El nombre descriptivo del actor que ejecutó la acción.
    /// </returns>
    string? GetCurrentActor();
}