using Dapper;
using System.Data;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Dapper.Services;

/// <summary>
/// Obtiene una transacción que permite operaciones de lectura y de escritura
/// sobre una base de datos.
/// </summary>
public class DapperTransaction : AsyncDisposable, ICrudReadWriteTransaction
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;
    private readonly IDictionary<Type, DapperModelDescriptor> overrides;

    internal DapperTransaction(IDbConnection connection, IDictionary<Type, DapperModelDescriptor> overrides)
    {
        _connection = connection;
        _transaction = _connection.BeginTransaction();
        this.overrides = overrides;
    }

    /// <inheritdoc/>
    public QueryServiceResult<TModel> All<TModel>() where TModel : Model
    {
        try
        {
            return new QueryServiceResult<TModel>(_connection.Query<TModel>($"SELECT * FROM {GetTableName<TModel>()};", transaction: _transaction).AsQueryable());
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult> CommitAsync()
    {
        try
        {
            await Task.Run(_transaction.Commit);
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Commit()
    {
        try
        {
            _transaction.Commit();
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Create<TModel>(TModel newEntity) where TModel : Model
    {
        try
        {
            _connection.Execute($"INSERT INTO {GetTableName<TModel>()} ({EnumerateColumns<TModel>()}) VALUES ({string.Concat(",", EnumerateProps<TModel>("@"))});", newEntity, _transaction);
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Delete<TModel>(TModel entity) where TModel : Model
    {
        try
        {
            _connection.Execute($"DELETE FROM {GetTableName<TModel>()} WHERE {GetProp<TModel>("Id")} = @Id;", entity, _transaction);
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Delete<TModel, TKey>(TKey key)
        where TModel : Model<TKey>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        try
        {
            _connection.Execute($"DELETE FROM {GetTableName<TModel>()} WHERE {GetProp<TModel>("Id")} = @Id;", new { Id = key }, _transaction);
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key)
        where TModel : Model<TKey>
        where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
    {
        try
        {
            return await _connection.QuerySingleOrDefaultAsync<TModel>($"SELECT * FROM {GetTableName<TModel>()} WHERE {GetProp<TModel>("Id")} = @Id", new { Id = key }, _transaction);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Update<TModel>(TModel entity) where TModel : Model
    {
        try
        {
            _connection.Execute($"UPDATE {GetTableName<TModel>()} SET { EnumColEqProp<TModel>() } WHERE {GetProp<TModel>("Id")} = @Id;", entity, _transaction);
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    protected override void OnDispose()
    {
        _connection.Dispose();
    }

    /// <inheritdoc/>
    protected override ValueTask OnDisposeAsync()
    {
        _connection.Dispose();
        return ValueTask.CompletedTask;
    }

    private string GetTableName<TModel>() where TModel : Model
    {
        return ModelOverriden<TModel>()?.TableName ?? typeof(TModel).Name;
    }

    private IEnumerable<string> EnumerateColumns<TModel>(string prefix = "")
    {
        foreach (var j in EnumerateProps<TModel>())
        {
            yield return $"{prefix}{GetProp<TModel>(j)}";
        }
    }

    private string GetProp<TModel>(string prop)
    {
        return ModelOverriden<TModel>()?.Properties is { } d && d.TryGetValue(prop, out var k) ? k : prop;
    }

    private static IEnumerable<string> EnumerateProps<TModel>(string prefix = "")
    {
        return typeof(TModel).GetProperties().Where(p => p.CanRead && p.CanWrite).Select(p => $"{prefix}{p.Name}");
    }

    private string EnumColEqProp<TModel>()
    {
        return string.Join(",", EnumerateColumns<TModel>().Zip(EnumerateProps<TModel>()).Select((p, q) => $"{p} = @{q}"));
    }

    private DapperModelDescriptor? ModelOverriden<TModel>()
    {
        return overrides.TryGetValue(typeof(TModel), out var overrideInfo) ? overrideInfo : null;
    }
}
