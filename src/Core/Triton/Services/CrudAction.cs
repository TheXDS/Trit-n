namespace TheXDS.Triton.Services;

/// <summary>
/// Enumera las operaciones CRUD existentes.
/// </summary>
[Flags]
public enum CrudAction : byte
{
    /// <summary>
    /// Escritura de la información en la base de datos.
    /// </summary>
    Commit = 0,

    /// <summary>
    /// Crear una entidad.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Leer una entidad.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Actualizar una entidad.
    /// </summary>
    Update = 4,

    /// <summary>
    /// Eliminar una entidad.
    /// </summary>
    Delete = 8,

    /// <summary>
    /// Descartar cambios pendientes de escribir en la base de datos.
    /// </summary>
    Discard = 16,

    /// <summary>
    /// Enumerar directamente las entidades según una función de filtro.
    /// </summary>
    Query = 32,
}