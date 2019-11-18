using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
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
        protected readonly IConnectionConfiguration _configuration;

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
        protected static TResult ResultFromException<TResult>(Exception ex) where TResult : ServiceResult
        {
            return ex switch
            {
                null => throw new ArgumentNullException(nameof(ex)),
                TaskCanceledException _ => (TResult)NetworkFailure,
                DbUpdateConcurrencyException _ => (TResult)ConcurrencyFailure,
                DbUpdateException _ => (TResult)DbFailure,
                RetryLimitExceededException _ => (TResult)NetworkFailure,
                _ => (TResult)ex,
            };
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
        protected static Task<TResult> TryCallAsync<TResult>(Task<TResult> op) where TResult : ServiceResult
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
        protected static TResult TryCall<TResult>(Func<TResult> op) where TResult : ServiceResult
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
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudTransactionBase{T}"/>.
        /// </summary>
        /// <param name="configuration">
        ///     Configuración a utilizar para la transacción.
        /// </param>
        protected CrudTransactionBase(IConnectionConfiguration configuration) : this(configuration, new T())
        {
        }

        private protected CrudTransactionBase(IConnectionConfiguration configuration, T contextInstance)
        {
            _configuration = configuration;
            _context = contextInstance;
        }
    }
}