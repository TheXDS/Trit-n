using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models;

/// <summary>
/// Base class for models that contain security flags.
/// </summary>
/// <param name="granted">
/// Flags that describe the granted permissions.
/// </param>
/// <param name="revoked">
/// Flags that describe the revoked permissions.
/// </param>
public abstract class SecurityBase(PermissionFlags granted, PermissionFlags revoked) : Model<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityBase"/> class.
    /// </summary>
    protected SecurityBase() : this(default, default)
    {
    }

    /// <summary>
    /// Gets or sets the flags that describe the permissions granted to the
    /// security object that contains this entity.
    /// </summary>
    public PermissionFlags Granted { get; set; } = granted;

    /// <summary>
    /// Gets or sets the flags that describe the permissions denied to the
    /// security object that contains this entity.
    /// </summary>
    public PermissionFlags Revoked { get; set; } = revoked;
}
