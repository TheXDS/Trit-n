namespace TheXDS.Triton.Models;

/// <summary>
/// Defines permission flags that can be granted or denied to a
/// <see cref="SecurityObject"/>.
/// </summary>
[Flags]
public enum PermissionFlags : byte
{
    /// <summary>
    /// No permissions.
    /// </summary>
    None = 0,

    /// <summary>
    /// Visibility permission.
    /// </summary>
    View = 1,

    /// <summary>
    /// Read permission.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Permission to create new entities.
    /// </summary>
    Create = 4,

    /// <summary>
    /// Permission to update existing entities.
    /// </summary>
    Update = 8,

    /// <summary>
    /// Permission to delete entities.
    /// </summary>
    Delete = 16,

    /// <summary>
    /// Permission to export data. Also affects the ability to create reports.
    /// </summary>
    Export = 32,

    /// <summary>
    /// Exclusive access permission.
    /// </summary>
    Lock = 64,

    /// <summary>
    /// Elevation permission. Grants or denies the ability to request
    /// non-existent permissions.
    /// </summary>
    Elevate = 128,

    /// <summary>
    /// All read permissions.
    /// </summary>
    FullRead = View | Read,

    /// <summary>
    /// All write permissions.
    /// </summary>
    FullWrite = Create | Update | Delete,

    /// <summary>
    /// All common read and write permissions.
    /// </summary>
    ReadWrite = FullRead | FullWrite,

    /// <summary>
    /// Special permissions.
    /// </summary>
    Special = Export | Lock | Elevate,

    /// <summary>
    /// All possible permissions.
    /// </summary>
    All = byte.MaxValue
}
