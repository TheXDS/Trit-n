using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services;

/// <summary>
/// Defines the contract for types that allows performing write operations
/// based on transaction over a database.
/// </summary>
public interface ICrudWriteTransaction : IDisposableEx, IAsyncDisposable
{
    /// <summary>
    /// Creates a set of entities in the database.
    /// </summary>
    /// <typeparam name="TModel">
    /// Model type of the new entities.
    /// </typeparam>
    /// <param name="entities">
    /// A collection of entities to be added to the database.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Create<TModel>(params TModel[] entities) where TModel : Model => Create(entities.Cast<Model>().ToArray());

    /// <summary>
    /// Creates a set of entities in the database.
    /// </summary>
    /// <param name="entities">
    /// A collection of entities to be added to the database.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation 
    /// that has been executed.
    /// </returns>
    ServiceResult Create(params Model[] entities);

    /// <summary>
    /// Updates a set of entities in the database.
    /// </summary>
    /// <typeparam name="TModel">
    /// Model type of the updated entities.
    /// </typeparam>
    /// <param name="entities">
    /// A collection of entities to be updated in the database.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Update<TModel>(params TModel[] entities) where TModel : Model;

    /// <summary>
    /// Updates a set of entities in the database.
    /// </summary>
    /// <param name="entities">
    /// A collection of entities to be updated in the database.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Update(params Model[] entities);

    /// <summary>
    /// Deletes a set of entities from the database.
    /// </summary>
    /// <typeparam name="TModel">
    /// Model type of the deleted entities.
    /// </typeparam>
    /// <param name="entities">
    /// A collection of entities to be deleted from the database.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Delete<TModel>(params TModel[] entities) where TModel : Model;

    /// <summary>
    /// Deletes a set of entities from the database.
    /// </summary>
    /// <param name="entities">
    /// A collection of entities to be deleted from the database.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Delete(params Model[] entities);

    /// <summary>
    /// Deletes a set of entities from the database using their primary key
    /// values.
    /// </summary>
    /// <typeparam name="TModel">
    /// Model type of the deleted entities.
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Type of the field that identifies the entity.
    /// </typeparam>
    /// <param name="keys">
    /// Primary key values of the entities to be deleted from the database.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Delete<TModel, TKey>(params TKey[] keys) where TModel : Model<TKey>, new() where TKey : IComparable<TKey>, IEquatable<TKey>;

    /// <summary>
    /// Deletes a set of entities from the database using their primary key
    /// values as strings.
    /// </summary>
    /// <typeparam name="TModel">
    /// Model type of the deleted entities.
    /// </typeparam>
    /// <param name="stringKeys">
    /// Primary key values of the entities to be deleted from the database as
    /// strings.
    /// </param>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Delete<TModel>(params string[] stringKeys) where TModel : Model, new();

    /// <summary>
    /// Saves all pending changes of the current transaction.
    /// </summary>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Commit();

    /// <summary>
    /// Saves all pending changes asynchronously.
    /// </summary>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    Task<ServiceResult> CommitAsync();

    /// <summary>
    /// Discards all pending changes that are not saved in the database.
    /// </summary>
    /// <returns>
    /// The result reported by the underlying service for the operation
    /// that has been executed.
    /// </returns>
    ServiceResult Discard();
}