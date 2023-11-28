namespace TheXDS.Triton.Models.Base;

/// <summary>
/// Modelo para aquellas entidades que representen un objeto simple de catálogo.
/// </summary>
/// <typeparam name="T">
/// Tipo de campo llave a utilizar para este modelo.
/// </typeparam>
public abstract class CatalogModel<T> : Model<T> where T : IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// Obtiene la descripción del elemento de catálogo.
    /// </summary>
    public string? Description { get; set; }
}