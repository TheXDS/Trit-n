using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Clase que describe una transacción que permite realizar operaciones
    ///     asíncronas de lectura sobre un contexto de datos.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de contexto de datos a utilizar dentro de la transacción.
    /// </typeparam>
    public class CrudAsyncReadTransaction<T> : CrudTransactionBase<T>, IAsyncCrudReadTransaction where T : DbContext, new()
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudAsyncReadTransaction{T}"/>.
        /// </summary>
        /// <param name="configuration">
        ///     Configuración a utilizar para la transacción.
        /// </param>
        public CrudAsyncReadTransaction(IConnectionConfiguration configuration) : base(configuration)
        {
        }

        internal CrudAsyncReadTransaction(IConnectionConfiguration configuration, T context) : base(configuration, context)
        {

        }

        /// <summary>
        ///     Obtiene de forma asíncrona una entidad cuyo campo llave sea
        ///     igual al valor especificado.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     Tipo de campo llave de la entidad a obtener.
        /// </typeparam>
        /// <param name="key">
        ///     Llave de la entidad a obtener.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        public async Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var result = await TryCallAsync(DoReadAsync<TModel, TKey>(key));
            if (result)
            {
                _configuration.Notifier?.Notify(result.ReturnValue!, CrudAction.Read);
            }
            return result;
        }

        private async Task<ServiceResult<TModel?>> DoReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            using var ct = MakeTimeoutToken();
            var t = _context.FindAsync<TModel>(new object[] { key }, ct.Token);
            await t;
            if (t.IsFaulted) throw t.AsTask().Exception!;
            return new ServiceResult<TModel?>(t.Result);
        }
    }
}