using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services.Base;
using System.Linq;
using static TheXDS.Triton.Services.ServiceResult.FailureReason;
using System.Threading;

namespace TheXDS.Triton.Services
{
    public class CrudTransaction<T> : ICrudFullTransaction, IAsyncCrudFullTransaction where T : DbContext, new()
    {
        private readonly IConnectionConfiguration _configuration;
        private readonly T _context = new T();
        public CrudTransaction(IConnectionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ServiceResult Create<TModel>(TModel newEntity) where TModel : Model
        {
            return PerformOperation(_context.Add, newEntity) ?? ServiceResult.Ok;
        }
        public ServiceResult<TModel?> Read<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return ReadAsync<TModel,TKey>(key).GetAwaiter().GetResult();
        }
        public ServiceResult Update<TModel>(TModel entity) where TModel : Model
        {
            return PerformOperation(_context.Update, entity);
        }
        public ServiceResult Delete<TModel>(TModel entity) where TModel : Model
        {
            return PerformOperation(_context.Remove, entity);
        }
        public ServiceResult Delete<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return Read<TModel, TKey>(key).ReturnValue is TModel r ? Delete(r) : ValidationError;
        }
        public ServiceResult Commit()
        {
            return TryCallWrapExceptions(DoTrySaveAsync).GetAwaiter().GetResult();
        }










        /// <summary>
        ///     Obtiene un valor que indica si este objeto ha sido desechado.
        /// </summary>
        public bool Disposed { get; private set; } = false;

        /// <summary>
        ///     Libera los recursos utilizados por esta instancia.
        /// </summary>
        /// <param name="disposing">
        ///     Indica si deben liberarse los recursos administrados.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (_context.ChangeTracker.Entries().Any(p=>p.State != EntityState.Unchanged))
                    {
                        _context.SaveChanges();
                    }
                    _context.Dispose();
                }
                Disposed = true;
            }
        }

        /// <summary>
        ///     Libera los recursos utilizados por esta instancia.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Inserta una nueva entidad en la base de datos subyacente de
        ///     forma asíncrona.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Modelo de la entidad a insertar.
        /// </typeparam>
        /// <param name="newEntity">
        ///     Entidad a insertar.
        /// </param>
        /// <returns></returns>
        public Task<ServiceResult> CreateAsync<TModel>(TModel newEntity) where TModel : Model
        {
            return PerformOperationAsync(_context.Add, newEntity, CrudAction.Create);
        }

        public async Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            var result = await TryCallWrapExceptions(() => DoReadAsync<TModel, TKey>(key));
            if (result)
            {
                _configuration.Notifier?.Notify(result.ReturnValue, CrudAction.Read);
            }
            return result;
        }
        public Task<ServiceResult> UpdateAsync<TModel>(TModel entity) where TModel : Model
        {
            return PerformOperationAsync(_context.Update, entity, CrudAction.Update);
        }
        public Task<ServiceResult> DeleteAsync<TModel>(TModel entity) where TModel : Model
        {
            return PerformOperationAsync(_context.Remove, entity, CrudAction.Delete);
        }

        public async Task<ServiceResult> DeleteAsync<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return (await ReadAsync<TModel, TKey>(key)).ReturnValue is TModel r ? await DeleteAsync(r) : ValidationError;
        }






        private ServiceResult PerformOperation<TEntity>(Func<TEntity, EntityEntry<TEntity>> operation, TEntity entity) where TEntity : Model
        {
            return TryOp(operation, entity) ?? ServiceResult.Ok;
        }
        private async Task<ServiceResult> PerformOperationAsync<TEntity>(Func<TEntity, EntityEntry<TEntity>> operation, TEntity entity, CrudAction action) where TEntity : Model
        {
            var result = TryOp(operation, entity) ?? (await TryCallWrapExceptions(DoTrySaveAsync));
            if (result)
            {
                _configuration.Notifier?.Notify(entity, action);
            }
            return result;
        }
        private ServiceResult? TryOp<TEntity>(Func<TEntity, EntityEntry<TEntity>> operation, TEntity entity) where TEntity : class
        {
            try
            {
                operation(entity);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
            


        private CancellationTokenSource MakeTimeoutToken()
        {
            return new CancellationTokenSource(_configuration.ConnectionTimeout);
        }
        private async Task<ServiceResult> DoTrySaveAsync()
        {
            using var ct = MakeTimeoutToken();
            var t = _context.SaveChangesAsync(ct.Token);
            await t;
            if (t.Exception?.InnerException is { } r) throw r;
            return ServiceResult.Ok;
        }
        private async Task<ServiceResult<TModel?>> DoReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            using var ct = MakeTimeoutToken();
            var t = _context.FindAsync<TModel>(new object[] { key }, ct.Token);
            await t;
            if (t.IsFaulted) throw t.AsTask().Exception!;
            return new ServiceResult<TModel?>(t.Result);
        }
        private static Task<TServiceResult> TryCallWrapExceptions<TServiceResult>(Func<Task<TServiceResult>> op) where TServiceResult : ServiceResult
        {
            try
            {
                return op();
            }
            catch (TaskCanceledException)
            {
                return Task.FromResult((TServiceResult)NetworkFailure);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Task.FromResult((TServiceResult)ConcurrencyFailure);
            }
            catch (DbUpdateException)
            {
                return Task.FromResult((TServiceResult)DbFailure);
            }
            catch (Exception ex)
            {
                return Task.FromResult((TServiceResult)ex);
            }
        }
    }
}