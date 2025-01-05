using System.Diagnostics.CodeAnalysis;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.Services;

/// <summary>
/// Represents the result of a password verification check.
/// </summary>
/// <param name="valid">
/// Value indicating whether the provided credentials were valid.
/// </param>
/// <param name="loginCredential">
/// Credential that has been obtained for validation.
/// </param>
/// <remarks>
/// While underlying data services may return <see langword="true"/> when executing
/// data access operations, verifications should deliberately hide the reason why a user's
/// password verification failed.
/// </remarks>
public class VerifyPasswordResult(bool valid, LoginCredential? loginCredential)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyPasswordResult"/> class,
    /// indicating that the verification was successful.
    /// </summary>
    /// <param name="loginCredential">
    /// Reference to the credential that has been verified successfully.
    /// </param>
    public VerifyPasswordResult(LoginCredential loginCredential) : this(true, loginCredential)
    {
    }

    /// <summary>
    /// Gets an invalid result without a credential.
    /// </summary>
    public static VerifyPasswordResult Invalid => new(false, null);

    /// <summary>
    /// Gets a value indicating whether the provided credentials were valid.
    /// </summary>
    [MemberNotNullWhen(true, nameof(LoginCredential))]
    public bool Valid { get; } = valid;

    /// <summary>
    /// Gets a reference to the credential that has been obtained for validation.
    /// </summary>
    public LoginCredential? LoginCredential { get; } = loginCredential;
}
