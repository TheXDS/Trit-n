using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Define una serie de miembros a implementar por una clase que permita
/// escribir entradas de bitácora sobre los cambios ocurridos en una
/// entidad de datos.
/// </summary>
public interface IJournalMiddleware
{
    /// <summary>
    /// Escribe información sobre los cambios ocurridos en una entidad de
    /// datos.
    /// </summary>
    /// <param name="action">
    /// Acción que se ha ejecutado sobre la entidad.
    /// </param>
    /// <param name="entity">Entidad afectada.</param>
    /// <param name="settings">
    /// Opciones de configuración del Middleware de bitácora.
    /// </param>
    void Log(CrudAction action, IEnumerable<Model>? entity, JournalSettings settings);
}