using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models;

/// <summary>
/// Modelo que representa una sessión de usuario activa.
/// </summary>
public class Session : TimestampModel<Guid>
{
    /// <summary>
    /// Obtiene o establece la credencial para la cual se ha creado este objeto
    /// de sesión.
    /// </summary>
    public LoginCredential Credential { get; set; } = null!;

    /// <summary>
    /// Obtiene o establece un valor que indica la marca de tiempo del final de
    /// la sesión.
    /// </summary>
    /// <value>
    /// Si este valor se establece en <see langword="null"/>, se debe entender
    /// que la sesión sigue activa, siempre y cuando la diferencia entre la
    /// propiedad <see cref="TimestampModel{T}.Timestamp"/> y el instante
    /// actual no supere la cantidad de segundos indicada por la propiedad
    /// <see cref="TtlSeconds"/>.
    /// </value>
    public DateTime? EndTimestamp { get; set; }

    /// <summary>
    /// Obtiene o establece el tóken de sesión para esta entidad.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Obtiene o establece el tiempo de vida en segundos para esta sessión.
    /// </summary>
    /// <value>
    /// Si esta propiedad se establece en cero, se debe entender que la sesión
    /// no vencerá nunca.
    /// </value>
    public int TtlSeconds { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Session"/>.
    /// </summary>
    /// <param name="ttlSeconds">
    /// Tiempo de vida en horas para esta sesión.
    /// </param>
    /// <param name="token">Token de sesión a asociar con esta sesión.</param>
    /// <param name="timestamp">
    /// Marca de tiempo de creación de la sesión.
    /// </param>
    public Session(int ttlSeconds, string? token, DateTime timestamp) : base(timestamp)
    {
        TtlSeconds = ttlSeconds;
        Token = token;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Session"/>.
    /// </summary>
    public Session() : this(default, null, DateTime.Now)
    {
    }
}
