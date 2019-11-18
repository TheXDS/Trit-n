namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un servicio que
    ///     provea de funcionalidad extendida para generar transacciones de
    ///     lectura/escritura.
    /// </summary>
    public interface IService : IServiceBase<IServiceConfiguration>
    {
        /// <summary>
        ///     Obtiene una transacción que permite leer información de la base
        ///     de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite leer información de la base de 
        ///     datos.
        /// </returns>
        ICrudReadTransaction IServiceBase<IServiceConfiguration>.GetReadTransaction() => GetReadWriteTransaction();

        /// <summary>
        ///     Obtiene una transacción que permite escribir información en la
        ///     base de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite escribir información en la base de
        ///     datos.
        /// </returns>
        ICrudWriteTransaction IServiceBase<IServiceConfiguration>.GetWriteTransaction() => GetReadWriteTransaction();

        /// <summary>
        ///     Obtiene una transacción que permite leer y escribir información
        ///     en la base de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite leer y escribir información en la
        ///     base de datos.
        /// </returns>
        ICrudReadWriteTransaction GetReadWriteTransaction();
    }
}
