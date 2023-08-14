using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests.Services;

/// <summary>
/// Implementa una transacción rota que simula errores en el servicio.
/// </summary>
[ExcludeFromCodeCoverage]
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

    ServiceResult ICrudWriteTransaction.Create<TModel>(params TModel[] newEntity)
    {
        return FailureReason.ServiceFailure;
    }

    ServiceResult ICrudWriteTransaction.Delete<TModel>(params TModel[] entity)
    {
        return FailureReason.ServiceFailure;
    }

    ServiceResult ICrudWriteTransaction.Delete<TModel, TKey>(params TKey[] key)
    {
        return FailureReason.ServiceFailure;
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    Task<ServiceResult<TModel[]?>> ICrudReadTransaction.SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate)
    {
        return Task.FromResult((ServiceResult<TModel[]?>)FailureReason.ServiceFailure);
    }

    ServiceResult ICrudWriteTransaction.Update<TModel>(params TModel[] entity)
    {
        return FailureReason.ServiceFailure;
    }

    /// <inheritdoc/>
    public Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey>, new() where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
    {
        return Task.FromResult((ServiceResult<TModel?>)FailureReason.ServiceFailure);
    }

    ServiceResult ICrudWriteTransaction.CreateOrUpdate<TModel>(params TModel[] entities)
    {
        return FailureReason.ServiceFailure;
    }

    ServiceResult ICrudWriteTransaction.Delete<TModel>(params string[] stringKeys)
    {
        return FailureReason.ServiceFailure;
    }

    ServiceResult ICrudWriteTransaction.Discard()
    {
        return FailureReason.ServiceFailure;
    }
}
