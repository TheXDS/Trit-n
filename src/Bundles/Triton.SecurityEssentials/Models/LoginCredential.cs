namespace TheXDS.Triton.Models;

/// <summary>
/// Model representing an individual user with the ability to log in to a
/// system that requires authentication.
/// </summary>
public class LoginCredential : SecurityObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCredential"/> class.
    /// </summary>
    public LoginCredential() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCredential"/> class
    /// with the specified username and password hash.
    /// </summary>
    /// <param name="username">
    /// The login name to associate with the user.
    /// </param>
    /// <param name="passwordHash">
    /// The binary blob containing the precomputed hash used for authentication.
    /// </param>
    public LoginCredential(string username, byte[] passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Gets or sets the login name to associate with this entity.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Gets or sets the precomputed hash used for authentication.
    /// </summary>
    public byte[] PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether this credential is enabled or
    /// not.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a password change has been
    /// scheduled for this credential.
    /// </summary>
    public bool PasswordChangeScheduled { get; set; }

    /// <summary>
    /// Gets or sets the collection of active sessions for the user represented
    /// by this entity.
    /// </summary>
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Gets or sets the collection of registered two-factor authentication
    /// objects for the user.
    /// </summary>
    public virtual ICollection<MultiFactorEntry> RegisteredMfa { get; set; } = new List<MultiFactorEntry>();
}