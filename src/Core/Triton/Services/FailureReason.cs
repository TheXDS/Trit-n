namespace TheXDS.Triton.Services;

/// <summary>
/// Enumerates known failure causes for a service operation.
/// </summary>
public enum FailureReason
{
    /// <summary>
    /// Unknown failure.
    /// </summary>
    Unknown,
    /// <summary>
    /// Malicious API call attempt
    /// </summary>
    Tamper,
    /// <summary>
    /// Forbidden operation.
    /// </summary>
    Forbidden,
    /// <summary>
    /// General service failure.
    /// </summary>
    ServiceFailure,
    /// <summary>
    /// Network failure.
    /// </summary>
    NetworkFailure,
    /// <summary>
    /// Database error.
    /// </summary>
    DbFailure,
    /// <summary>
    /// Data validation error.
    /// </summary>
    ValidationError,
    /// <summary>
    /// Data concurrency error.
    /// </summary>
    ConcurrencyFailure,
    /// <summary>
    /// Entity not found.
    /// </summary>
    NotFound,
    /// <summary>
    /// The action would result in duplicate entities.
    /// </summary>
    EntityDuplication,
    /// <summary>
    /// Malformed query (client-side error)
    /// </summary>
    BadQuery,
    /// <summary>
    /// Query returns too much data.
    /// </summary>
    QueryOverLimit,
    /// <summary>
    /// The action has already been executed in a previous transaction.
    /// </summary>
    Idempotency
}