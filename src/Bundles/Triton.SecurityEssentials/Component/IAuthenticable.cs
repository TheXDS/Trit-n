namespace TheXDS.Triton.Component;

/// <summary>
/// Defines the contract for types that provide security management
/// functionality using an authentication provider.
/// </summary>
public interface IAuthenticable
{
    /// <summary>
    /// Gets the authentication broker associated with this instance.
    /// </summary>
    IAuthenticationBroker AuthenticationBroker { get; }
}