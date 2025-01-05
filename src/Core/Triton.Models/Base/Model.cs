namespace TheXDS.Triton.Models.Base;

/// <summary>
/// A base class for all Triton data models.
/// </summary>
public abstract class Model
{
    /// <summary>
    /// Gets the ID of the entity as a string.
    /// </summary>
    public abstract string IdAsString { get; }
}