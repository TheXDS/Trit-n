using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware;

/// <summary>
/// This interface defines a set of members to be implemented by a type that
/// serves as the Middleware for a Crud operation during a transaction.
/// </summary>
public interface ITransactionMiddleware
{
    /// <summary>
    /// Defines a series of actions to be performed before the Crud operation.
    /// </summary>
    /// <param name="action">
    /// The Crud action that will be executed.
    /// </param>
    /// <param name="entities">
    /// The entities on which the Crud action will be executed. For read or
    /// query operations, this parameter can be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="null"/> if the Middleware has been executed correctly,
    /// or a <see cref="ServiceResult"/> that describes a failure in case it
    /// occurs.
    /// </returns>
    ServiceResult? PrologueAction(CrudAction action, IEnumerable<ChangeTrackerItem>? entities) => null;

    /// <summary>
    /// Defines a series of actions to be performed after the Crud operation.
    /// </summary>
    /// <param name="action">
    /// The Crud action that was executed.
    /// </param>
    /// <param name="entities">
    /// The entities on which the Crud action was executed. For read or query
    /// operations, this parameter can be null.
    /// </param>
    /// <returns>
    /// <see langword="null"/> if the Middleware has been executed correctly,
    /// or a <see cref="ServiceResult"/> that describes a failure in case it
    /// occurs.
    /// </returns>
    ServiceResult? EpilogueAction(CrudAction action, IEnumerable<ChangeTrackerItem>? entities) => null;
}