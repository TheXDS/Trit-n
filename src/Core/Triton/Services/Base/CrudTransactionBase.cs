using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Models.Base;
using static TheXDS.MCART.Types.Extensions.TaskExtensions;
using static TheXDS.Triton.Services.FailureReason;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    ///     Clase base que permite definir transacciones de datos.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de contexto de datos a utilizar dentro de la transacción.
    /// </typeparam>
    public abstract class CrudTransactionBase<T> : Disposable where T : DbContext, new()
    {
        /// <summary>
        ///     Obtiene la configuración disponible para esta transacción.
        /// </summary>
        protected readonly ITransactionConfiguration _configuration;

        /// <summary>
        ///     Obtiene la instancia activa del contexto de datos a utilizar
        ///     para esta transacción.
        /// </summary>
        protected readonly T _context;

        /// <summary>
        ///     Elimina el estado administrado y libera los recursos no 
        ///     administrados utilizados por esta instancia.
        /// </summary>
        protected override void OnDispose()
        {
            _context.Dispose();
        }

        /// <summary>
        ///     Obtiene un <typeparamref name="TResult"/> que representa un
        ///     <see cref="ServiceResult"/> fallido a partir de la excepción
        ///     producida.
        /// </summary>
        /// <typeparam name="TResult">Tipo de resultado a devolver.</typeparam>
        /// <param name="ex">Excepción que se ha producido.</param>
        /// <returns>
        ///     Un resultado que representa y describe una falla en la 
        ///     operación solicitada.
        /// </returns>
        protected static TResult ResultFromException<TResult>(Exception ex) where TResult : ServiceResult, new()
        {
            return (ex switch
            {
                null => throw new ArgumentNullException(nameof(ex)),
                DataNotFoundException _ => NotFound,
                TaskCanceledException _ => NetworkFailure,
                DbUpdateConcurrencyException _ => ConcurrencyFailure,
                DbUpdateException _ => DbFailure,
                RetryLimitExceededException _ => NetworkFailure,
                _ => (ServiceResult)ex,
            }).CastUp<TResult>();
        }

        /// <summary>
        ///     Envuelve una llamada a una tarea en un contexto seguro que
        ///     obtendrá un resultado de error cuando se produzca una
        ///     excepción.
        /// </summary>
        /// <typeparam name="TResult">
        ///     Tipo de resultado de la tarea.
        /// </typeparam>
        /// <param name="op">
        ///     Tarea a ejecutar.
        /// </param>
        /// <returns>
        ///     El resultado generado por la tarea, o un 
        ///     <see cref="ServiceResult"/> que representa un error en la
        ///     operación.
        /// </returns>
        protected static Task<TResult> TryCallAsync<TResult>(Task<TResult> op) where TResult : ServiceResult, new()
        {
            try
            {
                return op.Throwable();
            }
            catch (Exception ex)
            {
                return Task.FromResult(ResultFromException<TResult>(ex));
            }
        }

        /// <summary>
        ///     Mapea el valor <see cref="EntityState"/> a su valor equivalente
        ///     de tipo <see cref="CrudAction"/>.
        /// </summary>
        /// <param name="state">
        ///     Valor a convertir.
        /// </param>
        /// <returns>
        ///     Un valor <see cref="CrudAction"/> equivalente al
        ///     <see cref="EntityState"/> especificado.
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
        ///     Envuelve una operación en un contexto seguro que obtendrá un
        ///     resultado de error cuando se produzca una excepción.
        /// </summary>
        /// <typeparam name="TResult">
        ///     Tipo de resultado de la operación.
        /// </typeparam>
        /// <param name="op">
        ///     Operación a ejecutar.
        /// </param>
        /// <returns>
        ///     El resultado generado por la operación, o un 
        ///     <see cref="ServiceResult"/> que representa un error en la
        ///     misma.
        /// </returns>
        protected static TResult TryCall<TResult>(Func<TResult> op) where TResult : ServiceResult, new()
        {
            try
            {
                return op();
            }
            catch (Exception ex)
            {
                return ResultFromException<TResult>(ex);
            }
        }

        /// <summary>
        ///     Envuelve una función que obtiene a una entidad en un contexto
        ///     seguro que obtendrá un resultado de error cuando se produzca
        ///     una excepción.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Tipo de modelo que la función devuelve.
        /// </typeparam>
        /// <param name="op">
        ///     Función que devuelve una entidad.
        /// </param>
        /// <param name="action">Acción Crud que se realiza.</param>
        /// <returns>
        ///     El resultado generado por la operación, o un 
        ///     <see cref="ServiceResult"/> que representa un error en la
        ///     misma.
        /// </returns>
        protected ServiceResult<TModel?> TryResultCall<TModel>(Func<TModel> op, CrudAction action) where TModel : Model
        {
            try
            {
                var r =  new ServiceResult<TModel?>(op());
                if (r)
                {
                    _configuration.Notifier?.Notify(r.ReturnValue!, action);
                }
                return r;
            }
            catch (Exception ex)
            {
                return ResultFromException<ServiceResult<TModel?>>(ex);
            }
        }

        /// <summary>
        ///     Envuelve una función que obtiene a una entidad en un contexto
        ///     seguro que obtendrá un resultado de error cuando se produzca
        ///     una excepción.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Tipo de modelo que la función devuelve.
        /// </typeparam>
        /// <param name="op">
        ///     Función que devuelve una entidad.
        /// </param>
        /// <param name="entity">
        ///     Entidad sobre la cual se está realizando una operación Crud.
        /// </param>
        /// <param name="action">Acción Crud que se realiza.</param>
        /// <returns>
        ///     El resultado generado por la operación, o un 
        ///     <see cref="ServiceResult"/> que representa un error en la
        ///     misma.
        /// </returns>
        protected ServiceResult TryResultCall<TModel>(Func<TModel, EntityEntry<TModel>> op, TModel entity, CrudAction action) where TModel : Model
        {
            try
            {
                op(entity);
                _configuration.Notifier?.Notify(entity, action);
                return ServiceResult.Ok;
            }
            catch (Exception ex)
            {
                return ResultFromException<ServiceResult<TModel?>>(ex);
            }
        }

        /// <summary>
        ///     Crea un token de cancelación que puede ser utilizado para
        ///     cancelar una tarea luego agotarse el tiempo de espera
        ///     configurado para las conexiones de datos.
        /// </summary>
        /// <returns>
        ///     Un token de cancelación de tarea que puede utilizarse para
        ///     detener una tarea.
        /// </returns>
        protected CancellationTokenSource MakeTimeoutToken()
        {
            return new CancellationTokenSource(_configuration.ConnectionTimeout);
        }

        /// <summary>
        ///     Ejecuta una operación Crud en un contexto verificado seguro con
        ///     preámbulo.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Tipo de modelo sobre el cual se ejecuta la operación Crud.
        /// </typeparam>
        /// <param name="action">Acción Crud que se realiza.</param>
        /// <param name="entity">
        ///     Función que devuelve una entidad.
        /// </param>
        /// <returns>
        ///     El resultado generado por la operación, o un 
        ///     <see cref="ServiceResult"/> que representa un error en la
        ///     misma.
        /// </returns>
        protected ServiceResult<TModel?> Checked<TModel>(CrudAction action, Func<TModel> entity) where TModel : Model
        {
            return _configuration.Preamble(action, entity())?.CastUp<ServiceResult<TModel?>>() ?? TryResultCall(entity, action);
        }

        /// <summary>
        ///     Ejecuta una operación Crud en un contexto verificado seguro con
        ///     preámbulo.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Tipo de modelo sobre el cual se ejecuta la operación Crud.
        /// </typeparam>
        /// <param name="action">Acción Crud que se realiza.</param>
        /// <param name="op">
        ///     Función que ejecuta la acción Crud.
        /// </param>
        /// <param name="entity">
        ///     Entidad sobre la cual se está realizando una operación Crud.
        /// </param>
        /// <returns>
        ///     El resultado generado por la operación, o un 
        ///     <see cref="ServiceResult"/> que representa un error en la
        ///     misma.
        /// </returns>
        protected ServiceResult Checked<TModel>(CrudAction action, Func<TModel, EntityEntry<TModel>> op, TModel entity) where TModel : Model
        {
            return _configuration.Preamble(action,entity)?.CastUp<ServiceResult<TModel?>>() ?? TryResultCall(op, entity, action);
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudTransactionBase{T}"/>.
        /// </summary>
        /// <param name="configuration">
        ///     Configuración a utilizar para la transacción.
        /// </param>
        protected CrudTransactionBase(ITransactionConfiguration configuration) : this(configuration, new T())
        {
        }

        private protected CrudTransactionBase(ITransactionConfiguration configuration, T contextInstance)
        {
            _configuration = configuration;
            _context = contextInstance;
        }
    }
}