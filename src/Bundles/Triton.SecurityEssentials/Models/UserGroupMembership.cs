using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models;

/// <summary>
/// Represents a user's membership in a group of users.
/// </summary>
public class UserGroupMembership : Model<Guid>
{
    /// <summary>
    /// Gets or sets the group that the user is a member of.
    /// </summary>
    public UserGroup Group { get; set; } = null!;

    /// <summary>
    /// Gets or sets the security object that is a member of the group.
    /// </summary>
    public SecurityObject Member { get; set; } = null!;
}