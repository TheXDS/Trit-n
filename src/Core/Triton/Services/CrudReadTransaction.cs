using Microsoft.EntityFrameworkCore;
using System;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services.Base;
using System.Threading.Tasks;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Clase que describe una transacción que permite realizar operaciones
    ///     de lectura sobre un contexto de datos.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de contexto de datos a utilizar dentro de la transacción.
    /// </typeparam>
    public class CrudReadTransaction<T> : CrudTransactionBase<T>, ICrudReadTransaction where T : DbContext, new()
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudReadTransaction{T}"/>.
        /// </summary>
        /// <param name="configuration">
        ///     Configuración a utilizar para la transacción.
        /// </param>
        public CrudReadTransaction(TransactionConfiguration configuration) : base(configuration)
        {
        }

        internal CrudReadTransaction(TransactionConfiguration configuration, T context) : base(configuration, context)
        {
        }

        /// <summary>
        ///     Obtiene la colección completa de entidades del modelo
        ///     especificado almacenadas en la base de datos.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de las entidades a obtener.
        /// </typeparam>
        /// <returns></returns>
        public QueryServiceResult<TModel> All<TModel>() where TModel : Model
        {
            var result = TryCall(CrudAction.Read, (Func<DbSet<TModel>>)_context.Set<TModel>, out DbSet<TModel> dbSet, null)?.CastUp<QueryServiceResult<TModel>>();
            return dbSet is null ? result ?? FailureReason.DbFailure : new QueryServiceResult<TModel>(dbSet);
        }

        /// <summary>
        ///     Obtiene una entidad cuyo campo llave sea igual al valor
        ///     especificado.
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
        ///     servicio subyacente, incluyendo como valor de resultado a la
        ///     entidad obtenida en la operación de lectura. Si no existe una
        ///     entidad con el campo llave especificado, el valor de resultado
        ///     será <see langword="null"/>.
        /// </returns>
        public ServiceResult<TModel?> Read<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var result = TryCall(CrudAction.Read, (Func<object[], TModel>)_context.Find<TModel>, out TModel? entity, new object?[] { new object[] { key } })?.CastUp<ServiceResult<TModel?>>();
            return  entity is null ? result ?? FailureReason.NotFound : new ServiceResult<TModel?>(entity);
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
        public Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return TryCallAsync(CrudAction.Read, _context.FindAsync<TModel>(new object[] { key }).AsTask());
        }




    }
}