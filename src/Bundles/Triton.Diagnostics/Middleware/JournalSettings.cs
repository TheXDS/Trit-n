namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Estructura que contiene los valores de configuración a utilizar
/// para cada escritor de bitácora.
/// </summary>
public struct JournalSettings
{
    /// <summary>
    /// Obtiene un objeto que se puede utilizar para identificar al
    /// actor que ha ejecutado la acción CRUD.
    /// </summary>
    public IActorProvider? ActorProvider { get; init; }

    /// <summary>
    /// Obtiene un objeto que se puede utilizar para extraer los
    /// valores anteriores de la entidad que ha sido afectada por la
    /// operación CRUD.
    /// </summary>
    public IOldValueProvider? OldValueProvider { get; init; }
}