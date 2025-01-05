using TheXDS.Triton.Services;

namespace TheXDS.Triton.CrudNotify;

/// <summary>
/// Defines a series of members to be implemented by a type that sends CRUD
/// notifications to other connected peers.
/// </summary>
public interface ICrudNotifier
{
    /// <summary>
    /// Sends a notification of a CRUD event to all connected peers.
    /// </summary>
    /// <param name="action">
    /// The CRUD action that was performed.
    /// </param>
    /// <param name="entity">
    /// The entity on which the CRUD operation was performed.
    /// </param>
    /// <returns>
    /// The result of a service operation.
    /// </returns>
    ServiceResult? NotifyPeers(CrudAction action, IEnumerable<ChangeTrackerItem>? entity);
}