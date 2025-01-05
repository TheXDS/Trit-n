namespace TheXDS.Triton.Services;

/// <summary>
/// Defines the contract for types that has the
/// capability to execute Middlewares before and after initiating CRUD actions.
/// </summary>
public interface IMiddlewareRunner
{
    /// <summary>
    /// Executes the Middlewares associated with the epilogue of a CRUD
    /// operation.
    /// </summary>
    /// <param name="action">The CRUD action executed.</param>
    /// <param name="entities">
    /// The entities on which the operation was executed.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult"/> if a middleware indicates that the
    /// operation should be cancelled, or <see langword="null"/> to indicate
    /// that the execution of the Middleware chain completed successfully.
    /// </returns>
    ServiceResult? RunEpilogue(in CrudAction action, IEnumerable<ChangeTrackerItem>? entities);

    /// <summary>
    /// Executes the Middlewares associated with the prologue of a CRUD
    /// operation.
    /// </summary>
    /// <param name="action">The CRUD action to be executed.</param>
    /// <param name="entities">
    /// The entities on which the operation will be executed.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult"/> if a middleware indicates that the
    /// operation should be cancelled, or <see langword="null"/> to indicate
    /// that the execution of the Middleware chain completed successfully.
    /// </returns>
    ServiceResult? RunPrologue(in CrudAction action, IEnumerable<ChangeTrackerItem>? entities);
}