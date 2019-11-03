namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Enumera las operaciones CRUD existentes.
    /// </summary>
    public enum CrudAction : byte
    {
        /// <summary>
        ///     Crear.
        /// </summary>
        Create,

        /// <summary>
        ///     Leer.
        /// </summary>
        Read,

        /// <summary>
        ///     Actualizar.
        /// </summary>
        Update,

        /// <summary>
        ///     Eliminar.
        /// </summary>
        Delete,
    }
}