using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.InMemory.Services;

/// <summary>
/// Fábrica de transacciones que creará transacciones que ejecutan acciones de
/// datos sobre un objeo de tipo <see cref="ICollection{T}"/>.
/// </summary>
/// <param name="store">
/// Colección a ser administrada por esta instancia.
/// </param>
public class CollectionTransFactory(ICollection<Model> store) : ITransactionFactory
{
    private readonly ICollection<Model> _store = store;

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
        return new InMemoryCrudTransaction(configuration, _store);
    }
}