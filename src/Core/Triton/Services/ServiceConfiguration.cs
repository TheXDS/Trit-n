using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Clase que implementa <see cref="IServiceConfiguration"/> para 
    ///     proveer de valores de configuración estándar.
    /// </summary>
    public class ServiceConfiguration : IServiceConfiguration
    {
        /// <summary>
        ///     Obtiene una referencia a la fábrica de transacciones
        ///     actualmente configurada.
        /// </summary>
        public ICrudTransactionFactory CrudTransactionFactory { get; private set; } = new DefaultTransactionFactory();

        /// <summary>
        ///     Obtiene una referencia a la configuración de transacciones
        ///     activa.
        /// </summary>
        public TransactionConfiguration TransactionConfiguration { get; private set; } = new TransactionConfiguration();

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
            CrudTransactionFactory = factory ?? throw new System.ArgumentNullException(nameof(factory));
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
        public ServiceConfiguration SetTransactionConfiguration(TransactionConfiguration transConfig)
        {
            TransactionConfiguration = transConfig ?? throw new System.ArgumentNullException(nameof(transConfig));
            return this;
        }
    }
}