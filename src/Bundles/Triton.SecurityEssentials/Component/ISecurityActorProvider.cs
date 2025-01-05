using TheXDS.Triton.Models;

namespace TheXDS.Triton.Component;

/// <summary>
/// Provides a way to retrieve information about the security principal
/// executing operations in the current application context.
/// </summary>
public interface ISecurityActorProvider
{
    /// <summary>
    /// Gets the security actor responsible for executing operations in the
    /// current application.
    /// </summary>
    /// <returns>
    /// The <see cref="SecurityObject"/> instance representing the current
    /// security actor, or <see langword="null"/> if not available.
    /// </returns>
    SecurityObject? GetCurrentActor();
}