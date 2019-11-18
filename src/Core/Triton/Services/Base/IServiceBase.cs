namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que exponga
    ///     funcionalidad básica de servicio de datos de Tritón.
    /// </summary>
    /// <typeparam name="TConfiguration"></typeparam>
    public interface IServiceBase<TConfiguration> where TConfiguration : IServiceConfigurationBase
    {
        /// <summary>
        ///     Obtiene una referencia a la configuración expuesta para el
        ///     servicio.
        /// </summary>
        TConfiguration ActiveSettings { get; }

        /// <summary>
        ///     Obtiene una transacción que permite leer información de la base
        ///     de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite leer información de la base de 
        ///     datos.
        /// </returns>
        ICrudReadTransaction GetReadTransaction();

        /// <summary>
        ///     Obtiene una transacción que permite escribir información en la
        ///     base de datos.
        /// </summary>
        /// <returns>
        ///     Una transacción que permite escribir información en la base de
        ///     datos.
        /// </returns>
        ICrudWriteTransaction GetWriteTransaction();
    }
}
