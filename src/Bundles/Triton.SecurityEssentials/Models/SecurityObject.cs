namespace TheXDS.Triton.Models;

/// <summary>
/// Base class for entities that support security descriptors and group
/// membership.
/// </summary>
/// <param name="granted">Flags describing the granted permissions.</param>
/// <param name="revoked">Flags describing the revoked permissions.</param>
public abstract class SecurityObject(PermissionFlags granted, PermissionFlags revoked) : SecurityBase(granted, revoked)
{
    /// <summary>
    /// Gets or sets a collection that defines the user group memberships for
    /// the current entity.
    /// </summary>
    public virtual ICollection<UserGroupMembership> Membership { get; set; } = [];

    /// <summary>
    /// Gets or sets a collection containing the available security descriptors
    /// for this entity.
    /// </summary>
    public virtual ICollection<SecurityDescriptor> Descriptors { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityObject"/> class.
    /// </summary>
    protected SecurityObject() : this(default, default)
    {
    }
}