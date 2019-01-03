using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Triton.Annotations;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Component
{
    /// <summary>
    ///     Define una serie de miembros a implementar por una clase que
    ///     permita reportar el estado y progreso de una operación al usuario.
    /// </summary>
    public interface IReporter
    {



    }

    public enum LogLevel
    {
        Message,
        Info,
        Warning,
        Error,
        Critical
    }

    public delegate void ProgressCallback(long? current, long? total, long? step);


    public interface IServiceEx : IService
    {
        OperationResult CheckIntegrity();
    }

    public interface IService
    {
        OperationResult Add<TModel>([NotNull] TModel newEntity) where TModel : ModelBase, new();
        Task<OperationResult> AddAsync<TModel>([NotNull] TModel newEntity) where TModel : ModelBase, new();
        IQueryable All(Type model);
        IQueryable All(Type model, out OperationResult fails);
        IQueryable<TModel> All<TModel>() where TModel : ModelBase, new();
        IQueryable<TModel> All<TModel>(out OperationResult fails) where TModel : ModelBase, new();
        Task<OperationResult<IQueryable>> AllAsync(Type model);
        Task<OperationResult<List<TModel>>> AllAsync<TModel>() where TModel : ModelBase, new();
        IQueryable<TModel> AllBase<TModel>() where TModel : class;
        IQueryable<TModel> AllBase<TModel>(out OperationResult fails) where TModel : class;
        OperationResult BulkAdd([ItemNotNull, NotNull] IEnumerable<object> entities);
        OperationResult BulkAdd<TModel>([ItemNotNull, NotNull] IEnumerable<TModel> entities) where TModel : ModelBase, new();
        Task<OperationResult> BulkAddAsync([ItemNotNull, NotNull] IEnumerable<object> entities);
        Task<OperationResult> BulkAddAsync<TModel>([ItemNotNull, NotNull] IEnumerable<TModel> entities) where TModel : ModelBase, new();
        OperationResult BulkDelete<TModel>([NotNull] IEnumerable<TModel> entities) where TModel : class, ISoftDeletable, new();
        Task<OperationResult> BulkDeleteAsync<TModel>([NotNull] IEnumerable<TModel> entities) where TModel : class, ISoftDeletable, new();
        OperationResult BulkPurge([NotNull] IEnumerable<object> entities);
        OperationResult BulkPurge<TModel>([NotNull] IEnumerable<TModel> entities) where TModel : ModelBase, new();
        Task<OperationResult> BulkPurgeAsync([NotNull] IEnumerable<object> entities);
        Task<OperationResult> BulkPurgeAsync<TModel>([NotNull] IEnumerable<TModel> entities) where TModel : ModelBase, new();
        bool ChangesPending();
        bool ChangesPending<TModel>([NotNull] TModel entity) where TModel : ModelBase, new();
        OperationResult Delete<TModel>([NotNull] TModel entity) where TModel : class, ISoftDeletable, new();
        Task<OperationResult> DeleteAsync<TModel>([NotNull] TModel entity) where TModel : class, ISoftDeletable, new();
        OperationResult<ModelBase<TKey>> Get<TKey>(Type tModel, in TKey id) where TKey : struct, IComparable<TKey>;
        OperationResult<TModel> Get<TModel, TKey>(in TKey id)
            where TModel : ModelBase<TKey>, new()
            where TKey : struct, IComparable<TKey>;
        Task<OperationResult<ModelBase<TKey>>> GetAsync<TKey>(Type tModel, TKey id) where TKey : struct, IComparable<TKey>;
        Task<OperationResult<TModel>> GetAsync<TModel, TKey>(TKey id)
            where TModel : ModelBase<TKey>, new()
            where TKey : struct, IComparable<TKey>;
        bool Handles([NotNull] Type tModel);
        bool Handles<TModel>() where TModel : class;
        OperationResult Purge<TModel>([NotNull] TModel entity) where TModel : ModelBase, new();
        Task<OperationResult> PurgeAsync<TModel>([NotNull] TModel entity) where TModel : ModelBase, new();
        Task<OperationResult> SaveAsync();
        OperationResult Undelete<TModel, TKey>(in TKey id)
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>;
        Task<OperationResult> UndeleteAsync<TModel, TKey>(in TKey id)
            where TModel : ModelBase<TKey>, ISoftDeletable, new()
            where TKey : struct, IComparable<TKey>;
    }
}