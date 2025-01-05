namespace TheXDS.Triton.Services;

/// <summary>
/// Enumerates existing CRUD operations.
/// </summary>
 [Flags]
public enum CrudAction : byte
{
    /// <summary>
    /// Write information to the database.
    /// </summary>
    Commit = 0,
    /// <summary>
    /// Create an entity.
    /// </summary>
    Create = 1,
    /// <summary>
    /// Read an entity.
    /// </summary>
    Read = 2,
    /// <summary>
    /// Update an entity.
    /// </summary>
    Update = 4,
    /// <summary>
    /// Delete an entity.
    /// </summary>
    Delete = 8,
    /// <summary>
    /// Discard pending changes to write in the database.
    /// </summary>
    Discard = 16,
    /// <summary>
    /// Enumerate entities directly according to a filter function.
    /// </summary>
    Query = 32,
}