using Microsoft.EntityFrameworkCore;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     fabricar transacciones asíncronas de lectura y escritura.
    /// </summary>
    public interface IAsyncCrudReadWriteTransactionFactory : IAsyncCrudTransactionFactory
    {
        /// <summary>
        ///     Fabrica una transacción asíncrona de lectura/escritura.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de contexto de datos a inicializar.
        /// </typeparam>
        /// <param name="configuration">
        ///     Configuración del servicio a utilizar para fabricar la
        ///     transacción.
        /// </param>
        /// <returns>
        ///     Una transacción desechable para lectura/escritura de datos.
        /// </returns>
        IAsyncCrudReadWriteTransaction ManufactureFullTransaction<T>(IConnectionConfiguration configuration) where T : DbContext, new();

        /// <summary>
        ///     Fabrica una transacción asíncrona de lectura.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de contexto de datos a inicializar.
        /// </typeparam>
        /// <param name="configuration">
        ///     Configuración del servicio a utilizar para fabricar la
        ///     transacción.
        /// </param>
        /// <returns>
        ///     Una transacción desechable para lectura de datos.
        /// </returns>
        IAsyncCrudReadTransaction IAsyncCrudTransactionFactory.ManufactureReadTransaction<T>(IConnectionConfiguration configuration)
        {
            return ManufactureFullTransaction<T>(configuration);
        }

        /// <summary>
        ///     Fabrica una transacción asíncrona de escritura.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de contexto de datos a inicializar.
        /// </typeparam>
        /// <param name="configuration">
        ///     Configuración del servicio a utilizar para fabricar la
        ///     transacción.
        /// </param>
        /// <returns>
        ///     Una transacción desechable para escritura de datos.
        /// </returns>
        IAsyncCrudWriteTransaction IAsyncCrudTransactionFactory.ManufactureWriteTransaction<T>(IConnectionConfiguration configuration)
        {
            return ManufactureFullTransaction<T>(configuration);
        }
    }
}
