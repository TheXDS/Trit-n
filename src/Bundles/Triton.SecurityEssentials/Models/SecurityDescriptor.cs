namespace TheXDS.Triton.Models;

/// <summary>
/// Model that represents a security descriptor indicating permissions granted
/// and/or denied to a security entity within a specific context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SecurityDescriptor"/>
/// class.
/// </remarks>
/// <param name="contextId">
/// Value indicating the context ID to which this security descriptor will
/// be applied.
/// </param>
/// <param name="granted">
/// Flags describing the granted permissions.
/// </param>
/// <param name="revoked">
/// Flags describing the revoked permissions.
/// </param>
public class SecurityDescriptor(string contextId, PermissionFlags granted, PermissionFlags revoked) : SecurityBase(granted, revoked)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityDescriptor"/>
    /// class.
    /// </summary>
    public SecurityDescriptor() : this(null!, default, default)
    {
    }

    /// <summary>
    /// Gets or sets the context ID to which the security flags of this entity
    /// will be applied.
    /// </summary>
    public string ContextId { get; set; } = contextId;
}
