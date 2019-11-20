using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services.Base;
using static TheXDS.Triton.Services.FailureReason;

namespace TheXDS.Triton.Services
{
    /// <summary>
    ///     Clase que describe una transacción que permite realizar operaciones
    ///     de escritura asíncronas sobre un contexto de datos.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de contexto de datos a utilizar dentro de la transacción.
    /// </typeparam>
    public class CrudAsyncWriteTransaction<T> : CrudTransactionBase<T>, IAsyncCrudWriteTransaction where T : DbContext, new()
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudAsyncWriteTransaction{T}"/>.
        /// </summary>
        /// <param name="configuration">
        ///     Configuración a utilizar para la transacción.
        /// </param>
        public CrudAsyncWriteTransaction(ITransactionConfiguration configuration) : base(configuration)
        {
        }

        internal CrudAsyncWriteTransaction(ITransactionConfiguration configuration, T context) : base(configuration, context)
        {
        }

        /// <summary>
        ///     Crea una nueva entidad en la base de datos de forma asíncrona.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la nueva entidad.
        /// </typeparam>
        /// <param name="newEntity">
        ///     Nueva entidad a agregar a la base de datos.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        public Task<ServiceResult> CreateAsync<TModel>(TModel newEntity) where TModel : Model
        {
            return TryOpAsync(_context.Add, newEntity);
        }

        /// <summary>
        ///     Elimina a una entidad de la base de datos de forma asíncrona.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a eliminar.
        /// </typeparam>
        /// <param name="entity">
        ///     Entidad que deberá ser eliminada de la base de datos.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        public Task<ServiceResult> DeleteAsync<TModel>(TModel entity) where TModel : Model
        {
            return TryOpAsync(_context.Remove, entity);
        }

        /// <summary>
        ///     Elimina a una entidad de la base de datos de forma asíncrona.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a eliminar.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     Tipo del campo llave que identifica a la entidad.
        /// </typeparam>
        /// <param name="key">
        ///     Llave de la entidad que deberá ser eliminada de la base de
        ///     datos.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        public Task<ServiceResult> DeleteAsync<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var e = _context.Find<TModel>(key);
            if (e is null) return Task.FromResult((ServiceResult)NotFound);
            return DeleteAsync(e);
        }

        /// <summary>
        ///     Actualiza de forma asíncrona los datos contenidos en una
        ///     entidad dentro de la base de datos.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a actualizar.
        /// </typeparam>
        /// <param name="entity">
        ///     Entidad que contiene la nueva información a escribir.
        /// </param>
        /// <returns>
        ///     El resultado reportado de la operación ejecutada por el
        ///     servicio subyacente.
        /// </returns>
        public Task<ServiceResult> UpdateAsync<TModel>(TModel entity) where TModel : Model
        {
            return TryOpAsync(_context.Update, entity);
        }

        private async Task<ServiceResult> TryOpAsync<TEntity>(Func<TEntity, EntityEntry<TEntity>> operation, TEntity entity) where TEntity : Model
        {
            return TryOp(operation, entity) ?? await DoTrySaveAsync();
        }

        private async Task<ServiceResult<int>> DoTrySaveAsync()
        {
            try
            {
                var r = _context.SaveChangesAsync();
                await r;
                if (r.IsFaulted) throw r.Exception ?? new Exception();
                return new ServiceResult<int>(r.Result);
            }
            catch (Exception ex)
            {
                return ResultFromException<ServiceResult<int>>(ex);
            }
        }

        private ServiceResult? TryOp<TEntity>(Func<TEntity, EntityEntry<TEntity>> operation, TEntity entity) where TEntity : Model
        {
            try
            {
                operation(entity);
                return null;
            }
            catch (Exception ex)
            {
                return ResultFromException<ServiceResult>(ex);
            }
        }
    }
}