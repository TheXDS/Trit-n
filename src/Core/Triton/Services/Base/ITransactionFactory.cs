namespace TheXDS.Triton.Services.Base;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que permita
/// definir la implementación de la generación de transacciones para ser
/// utilizadas por los servicios.
/// </summary>
public interface ITransactionFactory
{
    /// <summary>
    /// Fabrica una transacción que permite leer información de la base
    /// de datos.
    /// </summary>
    /// <param name="configuration">
    /// Configuración a utilizar al manufacturar una transacción.
    /// </param>
    /// <returns>
    /// Una transacción que permite leer información de la base de 
    /// datos.
    /// </returns>
    ICrudReadTransaction GetReadTransaction(IMiddlewareRunner configuration) => GetTransaction(configuration);

    /// <summary>
    /// Fabrica una transacción que permite escribir información en la
    /// base de datos.
    /// </summary>
    /// <param name="configuration">
    /// Configuración a utilizar al manufacturar una transacción.
    /// </param>
    /// <returns>
    /// Una transacción que permite escribir información en la base de
    /// datos.
    /// </returns>
    ICrudWriteTransaction GetWriteTransaction(IMiddlewareRunner configuration) => GetTransaction(configuration);

    /// <summary>
    /// Fabrica una transacción que permite leer y escribir información
    /// en la base de datos.
    /// </summary>
    /// <param name="configuration">
    /// Configuración a utilizar al manufacturar una transacción.
    /// </param>
    /// <returns>
    /// Una transacción que permite leer y escribir información en la
    /// base de datos.
    /// </returns>
    ICrudReadWriteTransaction GetTransaction(IMiddlewareRunner configuration);
}
