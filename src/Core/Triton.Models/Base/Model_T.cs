namespace TheXDS.Triton.Models.Base;

/// <summary>
/// A base class for all Triton models that expose a key field to be used as
/// the entity's ID.
/// </summary>
/// <typeparam name="T">The type of the key field in the entity.</typeparam>
public abstract class Model<T> : Model where T : IComparable<T>, IEquatable<T>
{
    /// <inheritdoc/>
    public sealed override string IdAsString => Id?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets or sets the key field of this entity.
    /// </summary>
    public T Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the model, without setting the value of
    /// the key field.
    /// </summary>
    /// <remarks>
    /// Use this constructor only when creating new entities or intentionally
    /// not specifying the ID of the entity.
    /// </remarks>
    protected Model()
    {
        Id = default!;
    }

    /// <summary>
    /// Initializes a new instance of the model, setting the value of the key
    /// field.
    /// </summary>
    /// <param name="id">The value of the key field to set.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="id"/> is null.
    /// </exception>
    /// <remarks>
    /// If you intentionally want to leave the ID in its default value (e.g.,
    /// for primitive numeric, string, or Guid types), use the parameterless
    /// constructor of <see cref="Model{T}"/>.
    /// </remarks>
    protected Model(T id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }
}
