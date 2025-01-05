namespace TheXDS.Triton.Models.Base;

/// <summary>
/// A model for entities that represent a simple catalog object.
/// </summary>
/// <typeparam name="T">
/// The type of key field to use for this model.
/// </typeparam>
public abstract class CatalogModel<T> : Model<T> where T : IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// Gets the description of the catalog item.
    /// </summary>
    public string? Description { get; set; }
}