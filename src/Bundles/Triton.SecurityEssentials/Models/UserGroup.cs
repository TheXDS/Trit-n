namespace TheXDS.Triton.Models;

/// <summary>
/// Represents a group of users that share certain faculties, properties, and security permissions.
/// </summary>
/// <param name="displayName">Display name for this entity.</param>
/// <param name="granted">
/// Flags that describe the granted permissions.
/// </param>
/// <param name="revoked">
/// Flags that describe the denied permissions.
/// </param>
public class UserGroup(string displayName, PermissionFlags granted, PermissionFlags revoked) : SecurityObject(granted, revoked)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserGroup"/> class with default values.
    /// </summary>
    public UserGroup() : this(string.Empty, default, default)
    {
    }

    /// <summary>
    /// Gets or sets the display name for this entity.
    /// </summary>
    public string DisplayName { get; set; } = displayName;
}
