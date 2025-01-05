namespace TheXDS.Triton.Services;

/// <summary>
/// Enumeration for the possible types of changes that a
/// <see cref="ChangeTrackerItem"/> can represent.
/// </summary>
public enum ChangeTrackerChangeType : byte
{
    /// <summary>
    /// Represents an entity without any changes.
    /// </summary>
    NoChange,
    /// <summary>
    /// Represents a new entity being created.
    /// </summary>
    Create,
    /// <summary>
    /// Represents an existing entity being updated.
    /// </summary>
    Update,
    /// <summary>
    /// Represents an existing entity being deleted.
    /// </summary>
    Delete
}