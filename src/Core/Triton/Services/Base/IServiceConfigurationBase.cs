namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que exponga
    ///     propiedades de configuración básicas utilizadas por todos los
    ///     servicios de Tritón.
    /// </summary>
    /// <typeparam name="TFactory">
    ///     Tipo de fábrica de transacciones a exponer.
    /// </typeparam>
    public interface IServiceConfigurationBase<TFactory> : IServiceConfigurationBase where TFactory : ILiteCrudTransactionFactory
    {
        /// <summary>
        ///     Obtiene un objeto que permite manufacturar transacciones Crud.
        /// </summary>
        TFactory CrudTransactionFactory { get; }

    }
}