using static TheXDS.Triton.Services.FailureReason;

namespace TheXDS.Triton.Services;

/// <summary>
/// Clase que describe una transacción que permite realizar operaciones
/// de escritura sobre un contexto de datos.
/// </summary>
/// <typeparam name="T">
/// Tipo de contexto de datos a utilizar dentro de la transacción.
/// </typeparam>
public class CrudWriteTransaction<T> : CrudTransactionBase<T>, ICrudWriteTransaction where T : DbContext, new()
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="CrudWriteTransaction{T}"/>.
    /// </summary>
    /// <param name="configuration">
    /// Configuración a utilizar para la transacción.
    /// </param>
    public CrudWriteTransaction(IMiddlewareRunner configuration) : base(configuration)
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

    /// <summary>
    /// Crea una nueva entidad en la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la nueva entidad.
    /// </typeparam>
    /// <param name="newEntity">
    /// Nueva entidad a agregar a la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    public ServiceResult Create<TModel>(TModel newEntity) where TModel : Model
    {
        return Perform(CrudAction.Create, _context.Add, newEntity);
    }

    /// <summary>
    /// Elimina a una entidad de la base de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a eliminar.
    /// </typeparam>
    /// <param name="entity">
    /// Entidad que deberá ser eliminada de la base de datos.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    public ServiceResult Delete<TModel>(TModel entity) where TModel : Model
    {
        return Perform(CrudAction.Delete, _context.Remove, entity);
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

    /// <summary>
    /// Actualiza los datos contenidos en una entidad dentro de la base
    /// de datos.
    /// </summary>
    /// <typeparam name="TModel">
    /// Modelo de la entidad a actualizar.
    /// </typeparam>
    /// <param name="entity">
    /// Entidad que contiene la nueva información a escribir.
    /// </param>
    /// <returns>
    /// El resultado reportado de la operación ejecutada por el
    /// servicio subyacente.
    /// </returns>
    public ServiceResult Update<TModel>(TModel entity) where TModel : Model
    {
        return Perform(CrudAction.Update, _context.Update, entity);
    }
}