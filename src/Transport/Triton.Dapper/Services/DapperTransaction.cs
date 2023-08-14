using Dapper;
using System.Data;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
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
    private readonly IMiddlewareRunner runner;

    internal DapperTransaction(IDbConnection connection, IDictionary<Type, DapperModelDescriptor> overrides, IMiddlewareRunner runner)
    {
        _connection = connection;
        _transaction = _connection.BeginTransaction();
        this.overrides = overrides;
        this.runner = runner;
    }

    /// <inheritdoc/>
    public QueryServiceResult<TModel> All<TModel>() where TModel : Model
    {
        try
        {
            return new QueryServiceResult<TModel>(_connection.Query<TModel>($"SELECT * FROM {GetTableName<TModel>()};", transaction: _transaction, buffered: false).AsQueryable());
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
    public ServiceResult Create<TModel>(params TModel[] newEntities) where TModel : Model
    {
        try
        {
            var tblName = GetTableName<TModel>();
            var columns = string.Concat(",", EnumerateColumns<TModel>());
            var values = string.Concat(",", EnumerateProps<TModel>("@"));
            foreach (var entity in newEntities)
            {
                _connection.Execute($"INSERT INTO {tblName} ({columns}) VALUES ({values});", entity, _transaction);
            }
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Delete<TModel>(params TModel[] entities) where TModel : Model
    {
        try
        {
            var tblName = GetTableName<TModel>();
            var idColumn = GetProp<TModel>("Id");
            foreach (var entity in entities)
            {
                _connection.Execute($"DELETE FROM {tblName} WHERE {idColumn} = @Id;", entity, _transaction);
            }
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Delete<TModel, TKey>(params TKey[] keys)
        where TModel : Model<TKey>, new()
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        return Delete<TModel>(keys.Select(p => p.ToString()).NotNull().ToArray());
    }

    /// <inheritdoc/>
    public ServiceResult Delete<TModel>(params string[] stringKeys) where TModel : Model, new()
    {
        try
        {
            var tblName = GetTableName<TModel>();
            var idColumn = GetProp<TModel>("Id");
            foreach (var key in stringKeys)
            {
                _connection.Execute($"DELETE FROM {tblName} WHERE {idColumn} = @Id;", new { Id = key }, _transaction);
            }
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key)
        where TModel : Model<TKey>, new()
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
    public ServiceResult CreateOrUpdate<TModel>(params TModel[] entities) where TModel : Model
    {
        try
        {
            var tblName = GetTableName<TModel>();
            var createColumns = string.Concat(",", EnumerateColumns<TModel>());
            var createValues = string.Concat(",", EnumerateProps<TModel>("@"));
            var updateColumns = EnumColEqProp<TModel>();
            var idColumn = GetProp<TModel>("Id");
            foreach (var entity in entities)
            {
                _connection.Execute($@"IF NOT EXISTS ( SELECT TOP 1 FROM {tblName} WHERE {idColumn} = @Id )
INSERT INTO {tblName} ({createColumns}) VALUES ({createValues});
ELSE UPDATE {tblName} SET {updateColumns} WHERE {idColumn} = @Id;", entity, _transaction);
            }
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Update<TModel>(params TModel[] entities) where TModel : Model
    {
        try
        {
            var tblName = GetTableName<TModel>();
            var columns = EnumColEqProp<TModel>();
            var idColumn = GetProp<TModel>("Id");
            foreach (var entity in entities)
            {
                _connection.Execute($"UPDATE {tblName} SET { columns } WHERE {idColumn} = @Id;", entity, _transaction);
            }
            return ServiceResult.Ok;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public ServiceResult Discard()
    {
        _transaction.Rollback();
        return ServiceResult.Ok;
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
