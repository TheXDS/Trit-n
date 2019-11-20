using TheXDS.Triton.Services.Base;
using TheXDS.Triton.Exceptions;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Clase que implementa <see cref="IServiceConfiguration"/> para 
    ///     proveer de valores de configuración estándar.
    /// </summary>
    public class ServiceConfiguration : IServiceConfiguration
    {
        ICrudTransactionFactory? _factory;
        ITransactionConfiguration? _transConfig;

        /// <summary>
        ///     Obtiene una referencia a la fábrica de transacciones
        ///     actualmente configurada.
        /// </summary>
        protected ICrudTransactionFactory CrudTransactionFactory => _factory ?? throw new UnconfiguredServiceException(this);

        /// <summary>
        ///     Obtiene una referencia a la configuración de transacciones
        ///     activa.
        /// </summary>
        protected ITransactionConfiguration TransactionConfiguration => _transConfig ?? throw new UnconfiguredServiceException(this);

        /// <summary>
        ///     Establece la fábrica de transacciones a exponer en esta
        ///     instancia de configuración.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns>
        ///     Esta misma instancia.
        /// </returns>
        public ServiceConfiguration SetFactory(ICrudTransactionFactory factory)
        {
            _factory = factory;
            return this;
        }

        /// <summary>
        ///     Establece la configuración de transacciones a exponer en esta
        ///     instancia de configuración.
        /// </summary>
        /// <param name="transConfig"></param>
        /// <returns>
        ///     Esta misma instancia.
        /// </returns>
        public ServiceConfiguration SetTransactionConfiguration(ITransactionConfiguration transConfig)
        {
            _transConfig = transConfig;
            return this;
        }

        ICrudTransactionFactory IServiceConfiguration.CrudTransactionFactory => CrudTransactionFactory;

        ICrudTransactionFactory IServiceConfigurationBase<ICrudTransactionFactory>.CrudTransactionFactory => CrudTransactionFactory;

        ITransactionConfiguration IServiceConfigurationBase.TransactionConfiguration => TransactionConfiguration;
    }
}