using TheXDS.Triton.Services;

namespace TheXDS.Triton.Diagnostics.Middleware;

/// <summary>
/// Defines a set of members to be implemented by a class that allows writing
/// log entries about changes occurred in a set of data entities.
/// </summary>
public interface IJournalMiddleware
{
    /// <summary>
    /// Writes information about the changes occurred in a set of data
    /// entities.
    /// </summary>
    /// <param name="action">The action performed on the entities.</param>
    /// <param name="changeSet">The affected entities.</param>
    /// <param name="settings">The journal settings.</param>
    void Log(CrudAction action, IEnumerable<ChangeTrackerItem>? changeSet, JournalSettings settings);
}