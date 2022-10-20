using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using TheXDS.MCART.Types.Base;
using static TheXDS.Triton.Services.FailureReason;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    /// Clase base que permite definir transacciones de datos.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de contexto de datos a utilizar dentro de la transacción.
    /// </typeparam>
    public abstract class CrudTransactionBase<T> : AsyncDisposable where T : DbContext, new()
    {
        /// <summary>
        /// Obtiene la configuración disponible para esta transacción.
        /// </summary>
        protected readonly IMiddlewareRunner _configuration;

        /// <summary>
        /// Obtiene la instancia activa del contexto de datos a utilizar
        /// para esta transacción.
        /// </summary>
        protected readonly T _context;

        /// <inheritdoc/>
        protected override void OnDispose()
        {
            _context.Dispose();
        }

        /// <inheritdoc/>
        protected override async ValueTask OnDisposeAsync()
        {
            await _context.DisposeAsync();
        }

        /// <summary>
        /// Obtiene un <see cref="ServiceResult"/> que representa un
        /// <see cref="ServiceResult"/> fallido a partir de la excepción
        /// producida.
        /// </summary>
        /// <param name="ex">Excepción que se ha producido.</param>
        /// <returns>
        /// Un resultado que representa y describe una falla en la 
        /// operación solicitada.
        /// </returns>
        protected static ServiceResult ResultFromException(Exception ex)
        {
            return ex switch
            {
                null => throw new ArgumentNullException(nameof(ex)),
                NullReferenceException _ => NotFound,
                TaskCanceledException _ => NetworkFailure,
                DbUpdateConcurrencyException _ => ConcurrencyFailure,
                DbUpdateException _ => DbFailure,
                RetryLimitExceededException _ => NetworkFailure,
                _ => ex,
            };
        }
     
        /// <summary>
        /// Mapea el valor <see cref="EntityState"/> a su valor equivalente
        /// de tipo <see cref="CrudAction"/>.
        /// </summary>
        /// <param name="state">
        /// Valor a convertir.
        /// </param>
        /// <returns>
        /// Un valor <see cref="CrudAction"/> equivalente al
        /// <see cref="EntityState"/> especificado.
        /// </returns>
        protected static CrudAction Map(EntityState state)
        {
            return state switch
            {
                EntityState.Deleted => CrudAction.Delete,
                EntityState.Modified => CrudAction.Update,
                EntityState.Added => CrudAction.Create,
                _ => CrudAction.Read
            };
        }

        /// <summary>
        /// Envuelve una operación en un contexto seguro que obtendrá un
        /// resultado de error cuando se produzca una excepción.
        /// </summary>
        /// <typeparam name="TResult">
        /// Tipo de resultado de la operación.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Operación a ejecutar.
        /// </param>
        /// <param name="result">
        /// Resultado de la operación.
        /// </param>
        /// <param name="args">
        /// Argumentos a pasar a la operación.
        /// </param>
        /// <returns>
        /// El resultado generado por la operación, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// misma.
        /// </returns>
        protected ServiceResult? TryCall<TResult>(CrudAction action, Delegate op, out TResult result, params object?[]? args)
        {
            result = default!;
            try
            {
                if (_configuration.RunProlog(action, args?[0] as Model) is { } r) return r;

                if (op.Method.ReturnType == typeof(void))
                {
                    op.DynamicInvoke(args);
                }
                else if (typeof(TResult).IsAssignableFrom(op.Method.ReturnType))
                {
                    result = (TResult)op.DynamicInvoke(args)!;
                }
                else
                {
                    throw new InvalidCastException();
                }
                return _configuration.RunEpilog(action, GetFromResult(result));
            }
            catch (InvalidCastException)
            { 
                throw;
            }
            catch (Exception ex)
            {
                return ResultFromException(ex);
            }
        }

        /// <summary>
        /// Envuelve una operación en un contexto seguro que obtendrá un
        /// resultado de error cuando se produzca una excepción.
        /// </summary>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Operación a ejecutar.
        /// </param>
        /// <param name="args">
        /// Argumentos a pasar a la operación.
        /// </param>
        /// <returns>
        /// El resultado generado por la operación, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// misma.
        /// </returns>
        protected ServiceResult? TryCall(CrudAction action, Delegate op, params object?[]? args)
        {
            return TryCall<object?>(action, op, out _, args);
        }

        /// <summary>
        /// Envuelve una llamada a una tarea en un contexto seguro que
        /// obtendrá un resultado de error cuando se produzca una
        /// excepción.
        /// </summary>
        /// <typeparam name="TModel">
        /// Tipo de resultado de la tarea.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Tarea a ejecutar.
        /// </param>
        /// <param name="entity">
        /// Entidad que se ha pasado como argumento a la tarea. Si la tarea
        /// no recibe un <see cref="Model"/> como argumento, este valor 
        /// debe establecerse en <see langword="null"/>.
        /// </param>
        /// <returns>
        /// El resultado generado por la tarea, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// operación.
        /// </returns>
        protected async Task<ServiceResult<TModel?>> TryCallAsync<TModel>(CrudAction action, Task<TModel?> op, TModel? entity) where TModel : Model
        {
            try
            {
                if (_configuration.RunProlog(action, entity) is { } r) return r.CastUp<ServiceResult<TModel?>>();
                var result = await op;
                return _configuration.RunEpilog(action, result as Model ?? entity)?.CastUp<ServiceResult<TModel?>>()
                    ?? new ServiceResult<TModel?>(result ?? entity);
            }
            catch (InvalidCastException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return ResultFromException(ex).CastUp<ServiceResult<TModel?>>();
            }
        }

        /// <summary>
        /// Envuelve una llamada a una tarea en un contexto seguro que
        /// obtendrá un resultado de error cuando se produzca una
        /// excepción.
        /// </summary>
        /// <typeparam name="TModel">
        /// Tipo de resultado de la tarea.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Tarea a ejecutar.
        /// </param>
        /// <param name="entity">
        /// Entidad que se ha pasado como argumento a la tarea. Si la tarea
        /// no recibe un <see cref="Model"/> como argumento, este valor 
        /// debe establecerse en <see langword="null"/>.
        /// </param>
        /// <returns>
        /// El resultado generado por la tarea, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// operación.
        /// </returns>
        protected Task<ServiceResult> TryCallAsync<TModel>(CrudAction action, Task op, TModel? entity) where TModel : Model
        {
            return TryCallAsync(action, op, entity);
        }

        /// <summary>
        /// Envuelve una llamada a una tarea en un contexto seguro que
        /// obtendrá un resultado de error cuando se produzca una
        /// excepción.
        /// </summary>
        /// <typeparam name="TModel">
        /// Tipo de resultado de la tarea.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Tarea a ejecutar.
        /// </param>
        /// <returns>
        /// El resultado generado por la tarea, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// operación.
        /// </returns>
        protected Task<ServiceResult> TryCallAsync<TModel>(CrudAction action, Task op) where TModel : Model
        {
            return TryCallAsync<Model>(action, op, null);
        }

        /// <summary>
        /// Envuelve una llamada a una tarea en un contexto seguro que
        /// obtendrá un resultado de error cuando se produzca una
        /// excepción.
        /// </summary>
        /// <typeparam name="TModel">
        /// Tipo de resultado de la tarea.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Tarea a ejecutar.
        /// </param>
        /// <returns>
        /// El resultado generado por la tarea, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// operación.
        /// </returns>
        protected Task<ServiceResult<TModel?>> TryCallAsync<TModel>(CrudAction action, Task<TModel?> op) where TModel : Model
        {
            return TryCallAsync(action, op, null);
        }

        /// <summary>
        /// Envuelve una llamada a una tarea en un contexto seguro que
        /// obtendrá un resultado de error cuando se produzca una
        /// excepción.
        /// </summary>
        /// <typeparam name="TModel">
        /// Tipo de resultado de la tarea.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Tarea a ejecutar.
        /// </param>
        /// <returns>
        /// El resultado generado por la tarea, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// operación.
        /// </returns>
        protected Task<ServiceResult<TModel?>> TryCallAsync<TModel>(CrudAction action, ValueTask<TModel?> op) where TModel : Model
        {
            return TryCallAsync(action, op.AsTask(), null);
        }

        /// <summary>
        /// Envuelve una llamada a una tarea en un contexto seguro que
        /// obtendrá un resultado de error cuando se produzca una
        /// excepción.
        /// </summary>
        /// <typeparam name="TModel">
        /// Tipo de resultado de la tarea.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="op">
        /// Tarea a ejecutar.
        /// </param>
        /// <param name="entity">
        /// Entidad que se ha pasado como argumento a la tarea. Si la tarea
        /// no recibe un <see cref="Model"/> como argumento, este valor 
        /// debe establecerse en <see langword="null"/>.
        /// </param>
        /// <returns>
        /// El resultado generado por la tarea, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// operación.
        /// </returns>
        protected Task<ServiceResult<TModel?>> TryCallAsync<TModel>(CrudAction action, ValueTask<TModel?> op, TModel? entity) where TModel : Model
        {
            return TryCallAsync(action, op.AsTask(), entity);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudTransactionBase{T}"/>.
        /// </summary>
        /// <param name="configuration">
        /// Configuración a utilizar para la transacción.
        /// </param>
        protected CrudTransactionBase(IMiddlewareRunner configuration) : this(configuration, new T())
        {
        }

        /// <summary>
        /// Ejecuta una operación Crud de escritura.
        /// </summary>
        /// <typeparam name="TModel">
        /// Modelo utilizado por la operación.
        /// </typeparam>
        /// <param name="action">
        /// Acción Crud a ejecutar.
        /// </param>
        /// <param name="operation">
        /// Operación a ejecutar.
        /// </param>
        /// <param name="entity">
        /// Entidad sobre la cual se ejecutará la acción.
        /// </param>
        /// <returns>
        /// El resultado generado por la operación, o un 
        /// <see cref="ServiceResult"/> que representa un error en la
        /// misma.
        /// </returns>
        protected ServiceResult Perform<TModel>(CrudAction action, Func<TModel, EntityEntry<TModel>> operation, TModel entity) where TModel : Model
        {
            return TryCall(action, operation, new object?[] { entity }) ?? ServiceResult.Ok;
        }

        private static Model? GetFromResult(object? result)
        {
            return result switch
            {
                Model m => m,
                EntityEntry e => e.Entity as Model,
                IQueryable<Model> q => q.Count() == 1 ? q.Single() : null,
                _ => null
            };
        }

        private protected CrudTransactionBase(IMiddlewareRunner configuration, T contextInstance)
        {
            _configuration = configuration;
            _context = contextInstance;
        }
    }
}