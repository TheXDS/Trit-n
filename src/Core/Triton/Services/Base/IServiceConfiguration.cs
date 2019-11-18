namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que exponga
    ///     información de configuración para servicios que requieran
    ///     información adicional sobre la fábrica de transacciones a utilizar.
    /// </summary>
    public interface IServiceConfiguration : IServiceConfigurationBase<ICrudTransactionFactory>
    {
        /// <summary>
        ///     Obtiene un objeto que permite manufacturar transacciones Crud.
        /// </summary>
        new ICrudTransactionFactory CrudTransactionFactory { get; }
    }
}