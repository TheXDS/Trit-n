namespace TheXDS.Triton.Services;

/// <summary>
/// Fábrica de transacciones que permite conectarse a un contexto de datos
/// de Entity Framework Core.
/// </summary>
/// <typeparam name="T">
/// Tipo de contexto de datos al cual conectarse.
/// </typeparam>
public class EfCoreTransFactory<T> : ITransactionFactory where T : DbContext
{
    private readonly IDbContextOptionsSource options;

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="EfCoreTransFactory{T}"/>.
    /// </summary>
    /// <param name="options">
    /// Opciones de contexto a utilizar para crear instancias de
    /// <see cref="DbContext"/>.
    /// </param>
    public EfCoreTransFactory(IDbContextOptionsSource options)
    {
        this.options = options;
    }

    /// <summary>
    /// Obtiene una transacción que permite leer información desde el
    /// contexto de datos.
    /// </summary>
    /// <param name="runner">
    /// Confiuración de transacción a utilizar.
    /// </param>
    /// <returns>
    /// Una transacción que permite leer información desde el contexto de
    /// datos.
    /// </returns>
    public ICrudReadTransaction GetReadTransaction(IMiddlewareRunner runner)
    {
        return new CrudReadTransaction<T>(runner, options.GetOptions());
    }

    /// <summary>
    /// Obtiene una transacción que permite escribir información desde el
    /// contexto de datos.
    /// </summary>
    /// <param name="runner">
    /// Confiuración de transacción a utilizar.
    /// </param>
    /// <returns>
    /// Una transacción que permite escribir información desde el contexto
    /// de datos.
    /// </returns>
    public ICrudWriteTransaction GetWriteTransaction(IMiddlewareRunner runner)
    {
        return new CrudWriteTransaction<T>(runner, options.GetOptions());
    }

    /// <summary>
    /// Obtiene una transacción que permite leer y escribir información
    /// desde el contexto de datos.
    /// </summary>
    /// <param name="runner">
    /// Confiuración de transacción a utilizar.
    /// </param>
    /// <returns>
    /// Una transacción que permite leer y escribir información desde el 
    /// contexto de datos.
    /// </returns>
    public ICrudReadWriteTransaction GetTransaction(IMiddlewareRunner runner)
    {
        return new CrudTransaction<T>(runner, options.GetOptions());
    }
}
