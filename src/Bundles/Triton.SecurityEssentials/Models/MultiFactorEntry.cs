using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models;

/// <summary>
/// Represents an entry of two-factor authentication data.
/// </summary>
/// <param name="mfaProcessorId">
/// The <see cref="Guid"/> used to identify the MFA processor to use for
/// verifying this entity.
/// </param>
/// <param name="data">
/// A binary blob of custom data to be used by the two-factor
/// authentication processor.
/// </param>
public class MultiFactorEntry(Guid mfaProcessorId, byte[] data) : Model<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiFactorEntry"/> class.
    /// </summary>
    public MultiFactorEntry() : this(Guid.Empty, null!)
    {
    }

    /// <summary>
    /// Gets or sets the user who owns this two-factor authentication entry.
    /// </summary>
    public LoginCredential Credential { get; set; } = null!;

    /// <summary>
    /// Gets or sets the <see cref="Guid"/> used to identify the MFA processor
    /// to use for verifying this entity.
    /// </summary>
    public Guid MfaProcessor { get; set; } = mfaProcessorId;

    /// <summary>
    /// Gets or sets a binary blob of custom data to be used by the two-factor
    /// authentication processor.
    /// </summary>
    public byte[] Data { get; set; } = data;
}
