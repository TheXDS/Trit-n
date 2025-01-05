using System.Linq.Expressions;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services;

/// <summary>
/// Defines the contract for types that allows 
/// performing read operations on a database.
/// </summary>
public interface ICrudReadTransaction : IDisposableEx, IAsyncDisposable
{
    /// <summary>
    /// Generates a new Id that is guaranteed to be unique for the specified
    /// model.
    /// </summary>
    /// <typeparam name="TModel">
    /// Model for which to generate the new Id.
    /// </typeparam>
    /// <typeparam name="TKey">Type of the Id to generate.</typeparam>
    /// <param name="keyGenerator">
    /// Callback to use to generate a new key. The function will include a
    /// reference to the last Id that was generated.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service, including as a value
    /// the obtained unique Id that can be used to create a new entity.
    /// </returns>
    public async Task<ServiceResult<TKey>> GetUniqueIdAsync<TModel, TKey>(Func<TKey, TKey> keyGenerator) where TModel : Model<TKey>, new() where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
    {
        TKey id = default!;
        ServiceResult<TModel?> existingSearchResult;
        try
        {
            do
            {
                id = keyGenerator.Invoke(id);
                existingSearchResult = await ReadAsync<TModel, TKey>(id);
                if (!existingSearchResult.Success) return existingSearchResult.Reason;
            } while (existingSearchResult.Result is not null);
            return id;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Generates a new Id that is guaranteed to be unique for the specified
    /// model.
    /// </summary>
    /// <typeparam name="TModel">
    /// Model for which to generate the new Id.
    /// </typeparam>
    /// <typeparam name="TKey">Type of the Id to generate.</typeparam>
    /// <param name="keyGenerator">
    /// Callback to use to generate a new key. The function will include a
    /// reference to the last Id that was generated.
    /// </param>
    /// <returns></returns>
    public Task<ServiceResult<TKey>> GetUniqueIdAsync<TModel, TKey>(Func<TKey> keyGenerator) where TModel : Model<TKey>, new() where TKey : notnull, IComparable<TKey>, IEquatable<TKey>
    {
        return GetUniqueIdAsync<TModel, TKey>(_ => keyGenerator.Invoke());
    }

    /// <summary>
    /// Retrieves an entity whose key field matches the specified value.
    /// </summary>
    /// <typeparam name="TModel">
    /// Type of the entity to get.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Type of the key field of the entity to get.
    /// </typeparam>
    /// <param name="key">
    /// Key of the entity to get.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service, including as a value
    /// the obtained entity in the read operation. If there is no entity with
    /// the specified key field, the result will be <see langword="null"/>.
    /// </returns>
    ServiceResult<TModel?> Read<TModel, TKey>(TKey key) where TModel : Model<TKey>, new() where TKey : notnull, IComparable<TKey>, IEquatable<TKey>;

    /// <summary>
    /// Retrieves an entity whose key field matches the specified value.
    /// </summary>
    /// <typeparam name="TModel">
    /// Type of the entity to get.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Type of the key field of the entity to get.
    /// </typeparam>
    /// <param name="key">
    /// Key of the entity to get.
    /// </param>
    /// <param name="entity">
    /// Output parameter. Obtained entity in the read operation. If there is no
    /// entity with the specified key field, it will be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service.
    /// </returns>
    ServiceResult Read<TModel, TKey>(TKey key, out TModel? entity) where TModel : Model<TKey>, new() where TKey : notnull, IComparable<TKey>, IEquatable<TKey>;

    /// <summary>
    /// Retrieves an entity whose key field matches the specified value.
    /// </summary>
    /// <typeparam name="TModel">
    /// Type of the entity to get.
    /// </typeparam>
    /// <param name="key">
    /// Key of the entity to get.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service, including as a value the
    /// obtained entity in the read operation. If there is no entity with the
    /// specified key field, the result will be <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="key"/> is <see langword="null"/>.
    /// </exception>
    ServiceResult<TModel?> Read<TModel>(object key) where TModel : Model, new();

    /// <summary>
    /// Retrieves an entity whose key field matches the specified value.
    /// </summary>
    /// <typeparam name="TModel">
    /// Type of the entity to get.
    /// </typeparam>
    /// <param name="key">
    /// Key of the entity to get.
    /// </param>
    /// <param name="entity">
    /// Output parameter. Obtained entity in the read operation. If there is no
    /// entity with the specified key field, it will be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service, including as a value the
    /// obtained entity in the read operation. If there is no entity with the
    /// specified key field, the result will be <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="key"/> is <see langword="null"/>.
    /// </exception>
    ServiceResult Read<TModel>(object key, out TModel? entity) where TModel : Model, new()
    {
        var r = Read<TModel>(key);
        entity = r.Result;
        return r;
    }

    /// <summary>
    /// Retrieves an entity whose key field is equal to the specified value.
    /// </summary>
    /// <param name="model">The model of the entity to retrieve.</param>
    /// <param name="key">
    /// The key of the entity to retrieve.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service operation, including
    /// as a result value the retrieved entity in the read operation. If no
    /// entity exists with the specified key field, the result value will be
    /// <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="model"/> or <paramref name="key"/> are
    /// <see langword="null"/>.
    /// </exception>
    ServiceResult<Model?> Read(Type model, object key);

    /// <summary>
    /// Retrieves an entity whose key field is equal to the specified value.
    /// </summary>
    /// <param name="model">The model of the entity to retrieve.</param>
    /// <param name="key">
    /// The key of the entity to retrieve.
    /// </param>
    /// <param name="entity">
    /// Out parameter. Entity retrieved in the read operation. If no
    /// entity exists with the specified key field, null will be returned.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service operation, including
    /// as a result value the retrieved entity in the read operation. If no
    /// entity exists with the specified key field, the result value will be
    /// <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="model"/> or <paramref name="key"/> are
    /// <see langword="null"/>.
    /// </exception>
    ServiceResult Read(Type model, object key, out Model? entity)
    {
        var r = Read(model, key);
        entity = r.Result;
        return r;
    }

    /// <summary>
    /// Retrieves the complete collection of entities of the specified model
    /// stored in the database.
    /// </summary>
    /// <typeparam name="TModel">
    /// The model of the entities to retrieve.
    /// </typeparam>
    /// <returns>
    /// The result reported by the underlying service operation, along with an
    /// object of type <see cref="IQueryable{T}"/>
    /// that will enumerate the complete collection of entities of the
    /// specified model.
    /// </returns>
    QueryServiceResult<TModel> All<TModel>() where TModel : Model;

    /// <summary>
    /// Retrieves the complete collection of entities of the specified model
    /// stored in the database.
    /// </summary>
    /// <param name="model">
    /// The model of the entities to retrieve.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service operation, along with an
    /// object of type <see cref="IQueryable{T}"/>
    /// that will enumerate the complete collection of entities of the
    /// specified model.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="model"/> is null.
    /// </exception>
    QueryServiceResult<Model> All(Type model);

    /// <summary>
    /// Asynchronously retrieves an entity whose key field is equal to the
    /// specified value.
    /// </summary>
    /// <typeparam name="TModel">
    /// The model of the entity to retrieve.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// The type of the key field of the entity to retrieve.
    /// </typeparam>
    /// <param name="key">
    /// The key of the entity to retrieve.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service operation.
    /// </returns>
    Task<ServiceResult<TModel?>> ReadAsync<TModel, TKey>(TKey key) where TModel : Model<TKey>, new() where TKey : notnull, IComparable<TKey>, IEquatable<TKey>;

    /// <summary>
    /// Retrieves an entity whose key field is equal to the specified value.
    /// </summary>
    /// <typeparam name="TModel">
    /// The model of the entity to retrieve.
    /// </typeparam>
    /// <param name="key">
    /// The key of the entity to retrieve.
    /// </param>
    /// <returns>
    /// A task that, when completed, contains the result reported by the
    /// underlying service operation, including as a result value the retrieved
    /// entity in the read operation. If no entity exists with the specified
    /// key field, the result value will be <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="key"/> is null.
    /// </exception>
    Task<ServiceResult<TModel?>> ReadAsync<TModel>(object key) where TModel : Model, new();

    /// <summary>
    /// Retrieves an entity whose key field is equal to the specified value.
    /// </summary>
    /// <param name="model">
    /// The model of the entity to retrieve.
    /// </param>
    /// <param name="key">
    /// The key of the entity to retrieve.
    /// </param>
    /// <returns>
    /// A task that, when completed, contains the result reported by the
    /// underlying service operation, including as a result value the retrieved
    /// entity in the read operation. If no entity exists with the specified
    /// key field, the result value will be <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="model"/> or <paramref name="key"/> is
    /// <see langword="null"/>.
    /// </exception>
    Task<ServiceResult<Model?>> ReadAsync(Type model, object key);

    /// <summary>
    /// Executes a query that will retrieve an array of entities that match the
    /// predicate specified in <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of entities to retrieve.</typeparam>
    /// <param name="predicate">
    /// The filter function to apply when searching for entities.
    /// </param>
    /// <returns>
    /// A task that, when completed, contains the result reported by the
    /// underlying service operation, including as a result value the retrieved
    /// entities in the read operation. If no entities match the predicate
    /// specified in <paramref name="predicate"/>, an empty array of type
    /// <typeparamref name="TModel"/> will be returned.
    /// </returns>
    Task<ServiceResult<TModel[]?>> SearchAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : Model;
}
