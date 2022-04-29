using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests.Services
{
    /// <summary>
    /// Fábrica de transacciones que genera transacciones sin persistencia (en
    /// memoria).
    /// </summary>
    public class TestTransFactory : ITransactionFactory
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
            return InjectFailure ? new BrokenCrudTransaction() : new TestCrudTransaction(configuration);
        }

        /// <summary>
        /// Obtiene un valor que indica si se retornarán fallas en el servicio
        /// en lugar de un resultado desde el estado en memoria de la base de
        /// datos de prueba.
        /// </summary>
        public bool InjectFailure { get; set; }
    }
}
