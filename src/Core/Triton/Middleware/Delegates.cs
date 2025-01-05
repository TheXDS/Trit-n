using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware;

/// <summary>
/// Describes a method defined within a <see cref="CrudAction"/> and an
/// <see cref="ITransactionMiddleware"/> that accepts an input of type 
/// <see cref="ChangeTrackerItem"/> set, performs a service operation, and
/// returns a <see cref="ServiceResult"/> at the time of executing Crud
/// actions.
/// </summary>
/// <param name="crudAction">
/// The Crud action executed on the entities.
/// </param>
/// <param name="entities">
/// The entities over which a Crud action has been executed.
/// </param>
/// <returns>
/// <see langword="null"/> if the action was successful and the Crud operation
/// can continue normally, or a <see cref="ServiceResult"/> that describes the
/// error that occurred in the action.
/// </returns>
public delegate ServiceResult? MiddlewareAction(CrudAction crudAction, IEnumerable<ChangeTrackerItem>? entities);