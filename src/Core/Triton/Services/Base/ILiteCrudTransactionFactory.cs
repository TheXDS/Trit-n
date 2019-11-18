using Microsoft.EntityFrameworkCore;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     fabricar transacciones de lectura o de escritura.
    /// </summary>
    public interface ILiteCrudTransactionFactory
    {
        /// <summary>
        ///     Fabrica una transacción de lectura.
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
        ICrudReadTransaction ManufactureReadTransaction<T>(IConnectionConfiguration configuration) where T : DbContext, new();

        /// <summary>
        ///     Fabrica una transacción de escritura.
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
        ICrudWriteTransaction ManufactureWriteTransaction<T>(IConnectionConfiguration configuration) where T : DbContext, new();
    }
}
