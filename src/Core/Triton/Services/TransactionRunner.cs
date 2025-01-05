using TheXDS.Triton.Middleware;

namespace TheXDS.Triton.Services;

/// <summary>
/// Implements an execution engine for Middlewares.
/// </summary>
/// <param name="prologues">
/// A collection of pre-ordered prologues to execute.
/// </param>
/// <param name="epilogues">
/// A collection of pre-ordered epilogues to execute.
/// </param>
public sealed class TransactionRunner(IEnumerable<MiddlewareAction> prologues, IEnumerable<MiddlewareAction> epilogues) : IMiddlewareRunner
{
    private readonly IEnumerable<MiddlewareAction> _prologues = prologues;
    private readonly IEnumerable<MiddlewareAction> _epilogues = epilogues;

    /// <summary>
    /// Performs additional checks before executing a CRUD action, returning
    /// <see langword="null"/> if the operation can proceed.
    /// </summary>
    /// <param name="action">
    /// The CRUD action to be attempted.
    /// </param>
    /// <param name="entities">
    /// The entities on which the action will be executed.
    /// </param>
    /// <returns>
    /// A ServiceResult with the result of the prologue that has failed or
    /// <see langword="null"/> if the operation can proceed.
    /// </returns>
    public ServiceResult? RunPrologue(in CrudAction action, IEnumerable<ChangeTrackerItem>? entities) => Run(_prologues, action, entities);

    /// <summary>
    /// Performs additional checks after executing a CRUD action, returning 
    /// <see langword="null"/> if the operation can proceed.
    /// </summary>
    /// <param name="action">
    /// The CRUD action that has been executed.
    /// </param>
    /// <param name="entities">
    /// The entities on which the action has been executed.
    /// </param>
    /// <returns>
    /// A ServiceResult with the result of the epilogue that has failed or 
    /// <see langword="null"/> if the operation can proceed.
    /// </returns>
    public ServiceResult? RunEpilogue(in CrudAction action, IEnumerable<ChangeTrackerItem>? entities) => Run(_epilogues, action, entities);

    private static ServiceResult? Run(IEnumerable<MiddlewareAction> collection, in CrudAction action, IEnumerable<ChangeTrackerItem>? entities)
    {
        foreach (var j in collection)
        {
            if (j.Invoke(action, entities) is { } r) return r;
        }
        return null;
    }
}