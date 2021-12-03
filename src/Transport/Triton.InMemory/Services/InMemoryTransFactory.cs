using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.InMemory.Services
{
    /// <summary>
    /// Fábrica de transacciones que genera transacciones sin persistencia (en
    /// memoria).
    /// </summary>
    public class InMemoryTransFactory : ITransactionFactory
    {
        /// <summary>
        /// Fabrica una transaccion conectada a un almacén volátil sin
        /// persistencia en la memoria de la aplicación.
        /// </summary>
        /// <param name="configuration">
        /// Configuración de transacción a utilizar.
        /// </param>
        /// <returns>
        /// Una transacción conectada a un almacén volátil sin persistencia en
        /// la memoria de la aplicación.
        /// </returns>
        public ICrudReadWriteTransaction GetTransaction(IMiddlewareRunner configuration)
        {
            return new InMemoryCrudTransaction(configuration);
        }
    }
}
