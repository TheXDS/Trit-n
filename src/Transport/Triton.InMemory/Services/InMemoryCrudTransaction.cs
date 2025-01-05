using System.Linq.Expressions;
using System.Reflection;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.InMemory.Services;

/// <summary>
/// Representa una transacción de prueba que almacena los datos guardados
/// en la memoria de la aplicación. Los datos almacenados no se persistirán
/// y serán borrados al finalizar la ejecución.
/// </summary>
public class InMemoryCrudTransaction : AsyncDisposable, ICrudReadWriteTransaction
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="InMemoryCrudTransaction"/>.
    /// </summary>
    /// <param name="store">
    /// Collección de almacenamiento de datos.
    /// </param>
    public InMemoryCrudTransaction(ICollection<Model> store) : this(new TransactionConfiguration(), store)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="InMemoryCrudTransaction"/>.
    /// </summary>
    /// <param name="config">
    /// Configuración de transacciones a utilizar.
    /// </param>
    /// <param name="store">
    /// Collección de almacenamiento de datos.
    /// </param>
    public InMemoryCrudTransaction(IMiddlewareConfigurator config, ICollection<Model> store)
    {
        this.config = config;
        _store = store;
        config.Attach(_preconditionsCheckMiddleware = new PreconditionsCheckDefaultMiddleware(_store, _temp));
    }

    /// <summary>
    /// Libera los recursos desechables utilizados por esta instancia.
    /// </summary>
    protected override void OnDispose()
    {
        if (_temp.Count != 0) ((ICrudWriteTransaction)this).CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        config.Detach(_preconditionsCheckMiddleware);
    }

    /// <summary>
    /// Libera de forma asíncrona los recursos desechables utilizados por
    /// esta instancia.
    /// </summary>
    /// <returns>
    /// Un objeto que puede ser utilizado para monitorear el estado de la
    /// tarea.
    /// </returns>
    protected override async ValueTask OnDisposeAsync()
    {
        if (_temp.Count != 0) await ((ICrudWriteTransaction)this).CommitAsync().ConfigureAwait(false);
        config.Detach(_preconditionsCheckMiddleware);
    }

}
