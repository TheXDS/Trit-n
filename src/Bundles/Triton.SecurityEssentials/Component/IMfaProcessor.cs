using TheXDS.MCART.Types.Base;

namespace TheXDS.Triton.Component;

/// <summary>
/// Defines the contract for types that provide multi-factor authentication
/// (MFA) processing capabilities.
/// </summary>
public interface IMfaProcessor : IExposeGuid
{
    /// <summary>
    /// Validates the provided MFA data against the user's credentials.
    /// </summary>
    /// <param name="mfaData">The MFA data to be validated.</param>
    /// <returns>
    /// <see langword="true"/> if the MFA data is valid,
    /// <see langword="false"/> otherwise.
    /// </returns>
    /// <remarks>
    /// Implementations of this interface must provide a user interface to
    /// collect MFA data, such as one-time passwords (OTPs), hardware tokens,
    /// PINs, or biometric data.
    /// </remarks>
    bool ValidateMfaData(byte[] mfaData);
}