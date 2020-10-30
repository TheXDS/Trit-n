using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.InMemory.Services
{
    public class InMemoryCrudTransaction : AsyncDisposable, ICrudReadWriteTransaction
    {
        private static readonly List<Model> Models = new List<Model>();

        private readonly List<Model> _temp = new List<Model>();

        public InMemoryCrudTransaction(TransactionConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TransactionConfiguration Configuration { get; }

        public QueryServiceResult<TModel> All<TModel>() where TModel : Model
        {
            lock (((ICollection)Models).SyncRoot)
            {
                return new QueryServiceResult<TModel>(Models.Concat(_temp).Distinct().OfType<TModel>().AsQueryable());
            }
        }
        
        public Task<ServiceResult> CommitAsync()
        {
            Models.AddRange(_temp.Where(p => !Models.Contains(p)));
            _temp.Clear();
            return Task.FromResult(ServiceResult.Ok);
        }

        public ServiceResult Create<TModel>(TModel newEntity) where TModel : Model
        {
            if (Models.Concat(_temp).Contains(newEntity)) return FailureReason.EntityDuplication;
            _temp.Add(newEntity);
            return ServiceResult.Ok;
        }

        public ServiceResult Delete<TModel>(TModel entity) where TModel : Model
        {
            return !Models.Remove(entity) || _temp.Remove(entity) ? new ServiceResult(FailureReason.NotFound) : ServiceResult.Ok;
        }

        public ServiceResult Delete<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return Models.Concat(_temp).FirstOrDefault(p => p.IdAsString == key.ToString()) is TModel e
                ? Delete(e)
                : new ServiceResult(FailureReason.NotFound);
        }

        public Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key)
            where TModel : Model<TKey>
            where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
        {
            return Task.FromResult(Models.Concat(_temp).FirstOrDefault(p => p.IdAsString == key.ToString()) is TModel e
                ? new ServiceResult<TModel?>(e)
                : new ServiceResult<TModel?>(FailureReason.NotFound));
        }

        public ServiceResult Update<TModel>(TModel entity) where TModel : Model
        {
            return Models.Concat(_temp).Contains(entity) ? ServiceResult.Ok : new ServiceResult(FailureReason.NotFound);
        }

        protected override void OnDispose()
        {
            if (_temp.Any()) CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected override async ValueTask OnDisposeAsync()
        {
            if (_temp.Any()) await CommitAsync().ConfigureAwait(false);
        }
    }
}
