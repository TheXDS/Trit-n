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
    private readonly FailureReason reason;

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="BrokenCrudTransaction"/>.
    /// </summary>
    /// <param name="reason">
    /// Razón a devolver. De forma predeterminada, se devolverá
    /// <see cref="FailureReason.ServiceFailure"/>.
    /// </param>
    public BrokenCrudTransaction(FailureReason reason = FailureReason.ServiceFailure)
    {
        this.reason = reason;
    }

    /// <inheritdoc/>
    protected override void OnDispose()
    {
    }

    QueryServiceResult<TModel> ICrudReadTransaction.All<TModel>()
    {
        return reason;
    }

    Task<ServiceResult> ICrudWriteTransaction.CommitAsync()
    {
        return Task.FromResult((ServiceResult)reason);
    }

    ServiceResult ICrudWriteTransaction.Create<TModel>(params TModel[] newEntity)
    {
        return reason;
    }

    ServiceResult ICrudWriteTransaction.Delete<TModel>(params TModel[] entity)
    {
        return reason;
    }

    ServiceResult ICrudWriteTransaction.Delete<TModel, TKey>(params TKey[] key)
    {
        return reason;
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    Task<ServiceResult<TModel[]?>> ICrudReadTransaction.SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate)
    {
        return Task.FromResult((ServiceResult<TModel[]?>)reason);
    }

    ServiceResult ICrudWriteTransaction.Update<TModel>(params TModel[] entity)
    {
        return reason;
    }

    /// <inheritdoc/>
    public Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey>, new() where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
    {
        return Task.FromResult((ServiceResult<TModel?>)reason);
    }

    ServiceResult ICrudWriteTransaction.CreateOrUpdate<TModel>(params TModel[] entities)
    {
        return reason;
    }

    ServiceResult ICrudWriteTransaction.Delete<TModel>(params string[] stringKeys)
    {
        return reason;
    }

    ServiceResult ICrudWriteTransaction.Discard()
    {
        return reason;
    }
}
