using System.ComponentModel.DataAnnotations;

namespace TheXDS.Triton.Models.Base;

/// <summary>
/// A base class for all models that contain row version information to enable concurrency.
/// </summary>
/// <typeparam name="T">The type of the key field in the entity.</typeparam>
public abstract class ConcurrentModel<T> : Model<T> where T : IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// Implements a row version field to enable concurrency.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
}