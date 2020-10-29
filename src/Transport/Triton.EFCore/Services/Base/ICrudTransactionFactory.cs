using Microsoft.EntityFrameworkCore;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// fabricar transacciones de lectura y escritura.
    /// </summary>
    public interface ICrudTransactionFactory<T> where T : TransactionConfiguration
    {
        /// <summary>
        /// Fabrica una transacción de lectura/escritura.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de contexto de datos a inicializar.
        /// </typeparam>
        /// <param name="configuration">
        /// Configuración del servicio a utilizar para fabricar la
        /// transacción.
        /// </param>
        /// <returns>
        /// Una transacción desechable para lectura/escritura de datos.
        /// </returns>
        ICrudReadWriteTransaction ManufactureReadWriteTransaction<T>(TransactionConfiguration configuration) where T : DbContext, new()
        {
            return new CrudTransaction<T>(configuration);
        }

        /// <summary>
        /// Fabrica una transacción de lectura.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de contexto de datos a inicializar.
        /// </typeparam>
        /// <param name="configuration">
        /// Configuración del servicio a utilizar para fabricar la
        /// transacción.
        /// </param>
        /// <returns>
        /// Una transacción desechable para lectura de datos.
        /// </returns>
        ICrudReadTransaction ManufactureReadTransaction<T>(TransactionConfiguration configuration) where T : DbContext, new()
        {
            return ManufactureReadWriteTransaction<T>(configuration);
        }

        /// <summary>
        /// Fabrica una transacción de escritura.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de contexto de datos a inicializar.
        /// </typeparam>
        /// <param name="configuration">
        /// Configuración del servicio a utilizar para fabricar la
        /// transacción.
        /// </param>
        /// <returns>
        /// Una transacción desechable para escritura de datos.
        /// </returns>
        ICrudWriteTransaction ManufactureWriteTransaction<T>(TransactionConfiguration configuration) where T : DbContext, new()
        {
            return ManufactureReadWriteTransaction<T>(configuration);
        }
    }
}
