namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Defines the contract for types that exposes
/// information about an actor executing an operation.
/// </summary>
public interface IActorProvider
{
    /// <summary>
    /// Gets the descriptive name of the actor who executed the action.
    /// </summary>
    /// <returns>
    /// The descriptive name of the actor who executed the action.
    /// </returns>
    string? GetCurrentActor();
}