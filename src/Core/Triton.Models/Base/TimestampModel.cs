namespace TheXDS.Triton.Models.Base;

/// <summary>
/// A base model for entities that expose timestamp fields.
/// </summary>
/// <typeparam name="T">
/// The type of key field to use for this model.
/// </typeparam>
public abstract class TimestampModel<T> : Model<T> where T : IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// Gets or sets a timestamp associated with this entity.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Initializes a new instance of the class
    /// <see cref="TimestampModel{T}"/>.
    /// </summary>
    /// <param name="timestamp">
    /// The timestamp to associate with this entity.
    /// </param>
    public TimestampModel(DateTime timestamp)
    {
        Timestamp = timestamp;
    }

    /// <summary>
    /// Initializes a new instance of the class
    /// <see cref="TimestampModel{T}"/>.
    /// </summary>
    public TimestampModel()
    {
    }
}