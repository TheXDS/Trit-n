using TheXDS.MCART.Types.Extensions;
using static TheXDS.Triton.Services.FailureReason;

namespace TheXDS.Triton.Services;

/// <summary>
/// Clase que describe una transacción que permite realizar operaciones
/// de escritura sobre un contexto de datos.
/// </summary>
/// <typeparam name="T">
/// Tipo de contexto de datos a utilizar dentro de la transacción.
/// </typeparam>
public class CrudWriteTransaction<T> : CrudTransactionBase<T>, ICrudWriteTransaction where T : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="CrudWriteTransaction{T}"/>.
    /// </summary>
    /// <param name="configuration">
    /// Configuración a utilizar para la transacción.
    /// </param>
    /// <param name="options">
    /// Opciones a utilizar para llamar al contructor del contexto de datos.
    /// Establezca este parámetro en <see langword="null"/> para utilizar el
    /// constructor público sin parámetros.
    /// </param>
    public CrudWriteTransaction(IMiddlewareRunner configuration, DbContextOptions? options = null) : base(configuration, options)
    {
    }

    internal CrudWriteTransaction(IMiddlewareRunner configuration, T context) : base(configuration, context)
    {
    }

    /// <summary>
    /// Ejecuta tareas de limpieza antes de eliminar esta transacción.
    /// </summary>
    protected override void OnDispose()
    {
        if (_context.ChangeTracker.Entries().Any(p=>p.State != EntityState.Unchanged))
        {
            Commit();
        }
        base.OnDispose();
    }

    /// <summary>
    /// Guarda todos los cambios pendientes de la transacción actual.
    /// </summary>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    public ServiceResult Commit()
    {
        return TryCall(CrudAction.Commit, (Func<int>)_context.SaveChanges, null) ?? ServiceResult.Ok;
    }

    /// <summary>
    /// Guarda todos los cambios realizados de forma asíncrona.
    /// </summary>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    public Task<ServiceResult> CommitAsync()
    {
        return TryCallAsync<Model>(CrudAction.Commit, _context.SaveChangesAsync(), null) ?? Task.FromResult(ServiceResult.Ok);
    }

    /// <inheritdoc/>
    public ServiceResult Create<TModel>(params TModel[] entities) where TModel : Model
    {
        return TryCall(CrudAction.Create, _context.AddRange, entities);
    }

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <param name="entities">
    /// Entidad que deberá ser eliminada de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    public ServiceResult Delete<TModel>(params TModel[] entities) where TModel : Model
    {
        return TryCall(CrudAction.Delete, _context.RemoveRange, entities);
    }

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Tipo del campo llave que identifica a la entidad.
    /// </typeparam>
    /// <param name="key">
    /// Llave de la entidad que deberá ser eliminada de la base de
    /// datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    public ServiceResult Delete<TModel, TKey>(TKey key)
        where TModel : Model<TKey>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        var e = _context.Find<TModel>(key);
        if (e is null) return NotFound;
        return Delete(e);
    }

    /// <inheritdoc/>
    public ServiceResult Update<TModel>(params TModel[] entities) where TModel : Model
    {
        return TryCall(CrudAction.Update, _context.UpdateRange, entities);
    }

    /// <inheritdoc/>
    public ServiceResult CreateOrUpdate<TModel>(params TModel[] entities) where TModel : Model
    {
        return TryCall(CrudAction.Update | CrudAction.Create, e => {
            var groups = e.Cast<TModel>().GroupBy(p => _context.Set<TModel>().Contains(p) ? CrudAction.Update : CrudAction.Create).ToDictionary(p => p.Key);
            if (groups[CrudAction.Update].Any()) _context.UpdateRange(groups.Cast<object>().ToArray());
            if (groups[CrudAction.Create].Any()) _context.AddRange(groups.Cast<object>().ToArray());
        }, entities);
    }

    /// <inheritdoc/>
    public ServiceResult Delete<TModel, TKey>(params TKey[] keys)
        where TModel : Model<TKey>, new()
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        var entities = keys.Select(k => _context.Find<TModel>(k)).ToArray();
        if (entities.Any(p => p is null)) return NotFound;

        foreach (TModel entity in entities.NotNull())
        {
            if (Perform(CrudAction.Delete, _context.Remove, entity) is { } error) return error;
        }
        return ServiceResult.Ok;
    }

    /// <inheritdoc/>
    public ServiceResult Delete<TModel>(params string[] stringKeys) where TModel : Model, new()
    {
        var entities = stringKeys.Select(k => _context.Find<TModel>(k)).ToArray();
        if (entities.Any(p => p is null)) return NotFound;

        foreach (TModel entity in entities.NotNull())
        {
            if (Perform(CrudAction.Delete, _context.Remove, entity) is { } error) return error;
        }
        return ServiceResult.Ok;
    }

    /// <inheritdoc/>
    public ServiceResult Discard()
    {
        return TryCall(CrudAction.Discard, _context.ChangeTracker.Clear, null) ?? ServiceResult.Ok;
    }
}