namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que 
    ///     administre el acceso a un contexto de datos de Entity Framework por
    ///     medio de transacciones.
    /// </summary>
    public interface IService
    {
        /// <summary>
        ///     Obtiene una transacción que permite leer información de la base
        ///     de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite leer información de la base de 
        ///     datos.
        /// </returns>
        ICrudReadTransaction GetReadTransaction() => GetFullTransaction();

        /// <summary>
        ///     Obtiene una transacción que permite escribir información en la
        ///     base de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite escribir información en la base de
        ///     datos.
        /// </returns>
        ICrudWriteTransaction GetWriteTransaction() => GetFullTransaction();

        /// <summary>
        ///     Obtiene una transacción que permite leer y escribir información
        ///     en la base de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite leer y escribir información en la
        ///     base de datos.
        /// </returns>
        ICrudFullTransaction GetFullTransaction();
    }
}
