using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models;

/// <summary>
/// Represents an active user session.
/// </summary>
/// <param name="ttlSeconds">
/// Time to live in seconds for this session.
/// </param>
/// <param name="token">Token to associate with this session.</param>
/// <param name="timestamp">
/// Timestamp of when the session was created.
/// </param>
public class Session(int ttlSeconds, string? token, DateTime timestamp) : TimestampModel<Guid>(timestamp)
{
    /// <summary>
    /// Gets or sets the credential for which this session object was created.
    /// </summary>
    public LoginCredential Credential { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating the end timestamp of the session.
    /// </summary>
    /// <value>
    /// If this value is set to <see langword="null"/>, it means the session is still active,
    /// as long as the difference between the <see cref="TimestampModel{T}.Timestamp"/> and
    /// the current time does not exceed the number of seconds indicated by the <see cref="TtlSeconds"/> property.
    /// </value>
    public DateTime? EndTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the session token for this entity.
    /// </summary>
    public string? Token { get; set; } = token;

    /// <summary>
    /// Gets or sets the time to live in seconds for this session.
    /// </summary>
    /// <value>
    /// If this property is set to zero, it means the session will never expire.
    /// </value>
    public int TtlSeconds { get; set; } = ttlSeconds;

    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class with default values.
    /// </summary>
    public Session() : this(default, null, DateTime.Now)
    {
    }
}
