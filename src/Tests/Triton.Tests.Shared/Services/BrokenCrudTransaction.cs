using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.InMemory.Services
{
    /// <summary>
    /// Implementa una transacción rota que simula errores en el servicio.
    /// </summary>
    public class BrokenCrudTransaction : Disposable, ICrudReadWriteTransaction
    {
        /// <inheritdoc/>
        protected override void OnDispose()
        {
        }

        QueryServiceResult<TModel> ICrudReadTransaction.All<TModel>()
        {
            return FailureReason.ServiceFailure;
        }

        Task<ServiceResult> ICrudWriteTransaction.CommitAsync()
        {
            return Task.FromResult((ServiceResult)FailureReason.ServiceFailure);
        }

        ServiceResult ICrudWriteTransaction.Create<TModel>(TModel newEntity)
        {
            return FailureReason.ServiceFailure;
        }

        ServiceResult ICrudWriteTransaction.Delete<TModel>(TModel entity)
        {
            return FailureReason.ServiceFailure;
        }

        ServiceResult ICrudWriteTransaction.Delete<TModel, TKey>(TKey key)
        {
            return FailureReason.ServiceFailure;
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey> where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
        {
            return Task.FromResult((ServiceResult<TModel?>)FailureReason.ServiceFailure);
        }

        Task<ServiceResult<TModel[]?>> ICrudReadTransaction.SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate)
        {
            return Task.FromResult((ServiceResult<TModel[]?>)FailureReason.ServiceFailure);
        }

        ServiceResult ICrudWriteTransaction.Update<TModel>(TModel entity)
        {
            return FailureReason.ServiceFailure;
        }
    }
}
