using System.Linq.Expressions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests.Services;

/// <summary>
/// Representa una transacción de prueba que almacena los datos guardados
/// en la memoria de la aplicación. Los datos almacenados no se persistirán
/// y serán borrados al finalizar la ejecución.
/// </summary>
public class TestCrudTransaction : AsyncDisposable, ICrudReadWriteTransaction
{
    private class PreconditionsCheckDefaultMiddleware: ITransactionMiddleware
    {
        private readonly List<Model> _storeRef;
        private readonly List<Model> _tempRef;

        private readonly Dictionary<CrudAction, Func<IEnumerable<Model>?, ServiceResult?>> _prologPreconditions = new();
        
        public PreconditionsCheckDefaultMiddleware(List<Model> storeRef, List<Model> tempRef)
        {
            _storeRef = storeRef;
            _tempRef = tempRef;

            _prologPreconditions.Add(CrudAction.Create, e => 
                e.NotNull().Any(p => FindInternal(p.GetType(), p.IdAsString) is not null)
                ? (ServiceResult)FailureReason.EntityDuplication
                : null);

            _prologPreconditions.Add(CrudAction.Update, CheckExists);
            _prologPreconditions.Add(CrudAction.Delete, CheckExists);
        }

        private ServiceResult? CheckExists(IEnumerable<Model>? entities)
        {
            return entities.NotNull().All(p => FullSet(p.GetType()).Any(q => p.IdAsString == q.IdAsString))
                ? null
                : (ServiceResult)FailureReason.NotFound;
        }

        private Model? FindInternal(Type model, object key)
        {
            return FullSet(model).FirstOrDefault(p => p.IdAsString == key.ToString());
        }

        private IEnumerable<Model> FullSet(Type model)
        {
            return FullSet().OfType(model);
        }

        private IEnumerable<Model> FullSet()
        {
            return _storeRef.Concat(_tempRef);
        }

        ServiceResult? ITransactionMiddleware.PrologAction(CrudAction action, IEnumerable<Model>? entities) 
        {
            return _prologPreconditions.TryGetValue(action, out var func) ? func(entities) : null;
        }
    }

    private static readonly object _syncLock = new();
    private static readonly List<Model> _store = new();
    private readonly List<Model> _temp = new();

    private ServiceResult Execute(CrudAction action, Action operation, IEnumerable<Model>? middlewareData = null)
    {
        return Execute(action, () => { operation.Invoke(); return ServiceResult.Ok; }, middlewareData);
    }

    private TResult Execute<TResult>(CrudAction action, Func<TResult> operation, IEnumerable<Model>? middlewareData = null)
        where TResult : ServiceResult, new()
    {
        return Execute(action, () => (operation.Invoke(), middlewareData), middlewareData);
    }

    private TResult Execute<TResult>(CrudAction action, Func<(TResult, IEnumerable<Model>?)> operation, IEnumerable<Model>? prologData = null)
        where TResult : ServiceResult, new()
    {
        lock (_syncLock)
        {
            if (Configuration.RunProlog(action, prologData) is { } failure) return failure.CastUp<TResult>();
            var (result, epilogData) = operation.Invoke();
            return (result.Success ? Configuration.RunEpilog(action, epilogData)?.CastUp<TResult>() : null) ?? result;
        }
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="TestCrudTransaction"/>.
    /// </summary>
    /// <param name="configuration">
    /// Configuración de transacciones a utilizar.
    /// </param>
    public TestCrudTransaction(IMiddlewareRunner configuration)
    {
        configuration.Configurator.Attach(new PreconditionsCheckDefaultMiddleware(_store, _temp));
        Configuration = configuration;
    }

    /// <summary>
    /// Obtiene la configuración de transacciones que ha sido establecida
    /// en esta transacción.
    /// </summary>
    public IMiddlewareRunner Configuration { get; }

    /// <summary>
    /// Limpia la base de datos en memoria.
    /// </summary>
    public static void Wipe()
    {
        lock (_syncLock) _store.Clear();
    }

    /// <summary>
    /// Obtiene un <see cref="ServiceResult"/> con un Query de todas las
    /// entidades de la base de datos que corresponden a un modelo
    /// específico.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo a obtener desde la base de datos.
    /// </typeparam>
    /// <returns>
    /// Un <see cref="IQueryable{TModel}"/> que representa el Query de
    /// todas las entidades del modelo especificado desde la base de datos.
    /// </returns>
    public QueryServiceResult<TModel> All<TModel>() where TModel : Model
    {
        return Execute(CrudAction.Query, () =>
        {
            var r =  new QueryServiceResult<TModel>(FullSet<TModel>().AsQueryable());
            return (r, r);
        });
    }
    
    /// <summary>
    /// Guarda los cambios en la base de datos de forma asíncrona.
    /// </summary>
    /// <returns></returns>
    public Task<ServiceResult> CommitAsync()
    {
        return Task.Run(() => Execute(CrudAction.Commit, () => {
            foreach (var t in _temp.GroupBy(p => p.GetType()))
            {
                var newItemIds = _temp.OfType(t.Key).Select(p => p.IdAsString).ToArray();
                _store.RemoveAll(p => p.GetType() == t.Key && newItemIds.Contains(p.IdAsString));
            }
            _store.AddRange(_temp);
            var committed = _temp.ToArray();
            _temp.Clear();
            return ServiceResult.Ok;
        }));
    }

    /// <summary>
    /// Crea una nueva entidad en la base de datos.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la nueva entidad.</typeparam>
    /// <param name="newEntities">
    /// Nueva entidad a agregar a la base de datos.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
    /// la operación.
    /// </returns>
    public ServiceResult Create<TModel>(params TModel[] newEntities) where TModel : Model
    {
        return Execute(CrudAction.Create, () => _temp.AddRange(newEntities), newEntities);
    }

    /// <summary>
    /// Elimina una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
    /// <param name="entities">
    /// Entidad a eliminar de la base de datos.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
    /// la operación.
    /// </returns>
    public ServiceResult Delete<TModel>(params TModel[] entities) where TModel : Model
    {
        return Execute(CrudAction.Delete, () => {
            foreach (var entity in entities)
            {
                _ = _store.Remove(entity) || _temp.Remove(entity);
            }
        }, entities);
    }

    /// <summary>
    /// Elimina una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
    /// <typeparam name="TKey">
    /// Tipo del campo llave utilizado para identificar a la entidad.
    /// </typeparam>
    /// <param name="keys">
    /// Llaves de las entidades que deberán ser eliminadas de la base de datos.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
    /// la operación.
    /// </returns>
    public ServiceResult Delete<TModel, TKey>(params TKey[] keys)
        where TModel : Model<TKey>, new()
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        return Execute(CrudAction.Delete, () =>
        {
            var entities = FullSet<TModel>().Where(p => keys.Contains(p.Id)).ToArray();
            foreach (var entity in entities)
            {
                _ = _store.Remove(entity) || _temp.Remove(entity);
            }
            return (ServiceResult.Ok, entities);
        }, keys.Select(p => new TModel { Id = p }));
    }

    /// <summary>
    /// Elimina una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
    /// <param name="keys">
    /// Llaves de las entidades que deberán ser eliminadas de la base de datos.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
    /// la operación.
    /// </returns>
    public ServiceResult Delete<TModel>(params string[] keys)
        where TModel : Model, new()
    {
        return Execute(CrudAction.Delete, () =>
        {
            var entities = FullSet<TModel>().Where(p => keys.Contains(p.IdAsString));
            foreach (var entity in entities)
            {
                _ = _store.Remove(entity) || _temp.Remove(entity);
            }
            return (ServiceResult.Ok, entities);
        }, null);
    }

    /// <summary>
    /// Lee una entidad desde la base de datos.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
    /// <typeparam name="TKey">
    /// Tipo del campo llave utilizado para identificar a la entidad.
    /// </typeparam>
    /// <param name="key">
    /// Valor del campo llave utilizado para identificar a la entidad.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
    /// la operación, incluyendo a la entidad obtenida.
    /// </returns>
    public Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key)
        where TModel : Model<TKey>, new()
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        return Task.Run(() => Execute(CrudAction.Read, () => 
            FullSet<TModel>().FirstOrDefault(p => p.Id.Equals(key)) is { } e
            ? (new ServiceResult<TModel?>(e), new[] { e })
            : (FailureReason.NotFound, null), new Model[] { new TModel { Id = key } }));
    }

    /// <summary>
    /// Actualiza los datos de una entidad existente.
    /// </summary>
    /// <typeparam name="TModel">Modelo de la entidad.</typeparam>
    /// <param name="entities">
    /// Entidad con los nuevos datos a ser escritos. Debe existir en la
    /// base de datos una entidad con el mismo Id de este objeto.
    /// </param>
    /// <returns>
    /// Un <see cref="ServiceResult"/> que representa el éxito o fracaso de
    /// la operación.
    /// </returns>
    public ServiceResult Update<TModel>(params TModel[] entities) where TModel : Model
    {
        var oldEntities = entities.Select(p => FullSet<TModel>().First(q => p.IdAsString == q.IdAsString)).ToArray();
        return Execute(CrudAction.Update, () => {
            foreach (var (newData, oldData) in entities.Zip(oldEntities))
            {
                newData.ShallowCopyTo(oldData);
            }
            return (ServiceResult.Ok, entities);
        }, oldEntities);
    }

    /// <inheritdoc/>
    public ServiceResult CreateOrUpdate<TModel>(params TModel[] entities) where TModel : Model
    {
        return Execute(CrudAction.Create | CrudAction.Update, () =>
        {
            var oldEntities = entities.Select(p => FullSet<TModel>().FirstOrDefault(q => p.IdAsString == q.IdAsString)).NotNull().ToArray();
            foreach (var (newData, oldData) in entities.Zip(oldEntities))
            {
                newData.ShallowCopyTo(oldData);
            }
            _temp.AddRange(entities.Except(FullSet<TModel>()));
            return (ServiceResult.Ok, entities);
        }, entities);
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TModel[]?>> SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : Model
    {
        return (await _store.OfType<TModel>().AsQueryable().Where(predicate).ToListAsync()).ToArray();
    }

    /// <inheritdoc/>
    public ServiceResult Discard()
    {
        return Execute(CrudAction.Discard, () => _temp.Clear(), _temp);
    }

    /// <summary>
    /// Libera los recursos desechables utilizados por esta instancia.
    /// </summary>
    protected override void OnDispose()
    {
        if (!IsDisposed && _temp.Count != 0) CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
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
        if (!IsDisposed && _temp.Count != 0) await CommitAsync().ConfigureAwait(false);
    }

    private IEnumerable<TModel> FullSet<TModel>()
    {
        return _store.Concat(_temp).OfType<TModel>();
    }
}
