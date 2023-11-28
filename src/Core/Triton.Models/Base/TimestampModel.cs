namespace TheXDS.Triton.Models.Base;

/// <summary>
/// Modelo base para aquellas entidades que expongan campos de marca de tiempo.
/// </summary>
/// <typeparam name="T">
/// Tipo de campo llave a utilizar para este modelo.
/// </typeparam>
public abstract class TimestampModel<T> : Model<T> where T : IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// Obtiene o establece una marca de tiempo a asociar con esta entidad.
    /// </summary>
    /// <value></value>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="TimestampModel{T}"/>.
    /// </summary>
    /// <param name="timestamp">
    /// Marca de tiempo a asociar a esta entidad.
    /// </param>
    public TimestampModel(DateTime timestamp)
    {
        Timestamp = timestamp;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="TimestampModel{T}"/>.
    /// </summary>
    public TimestampModel()
    {
    }
}
