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
    Commit,

    /// <summary>
    /// Crear.
    /// </summary>
    Create,

    /// <summary>
    /// Leer.
    /// </summary>
    Read,

    /// <summary>
    /// Actualizar.
    /// </summary>
    Update,

    /// <summary>
    /// Eliminar.
    /// </summary>
    Delete,
}