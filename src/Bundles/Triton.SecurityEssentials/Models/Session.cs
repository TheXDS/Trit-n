namespace TheXDS.Triton.Models;
using System;

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
    /// Obtiene o establece el tóken de sesión para esta entidad.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Obtiene o establece el tiempo de vida en horas para esta sessión.
    /// </summary>
    public int TtlHours { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Session"/>.
    /// </summary>
    /// <param name="ttlHours">
    /// Tiempo de vida en horas para esta sesión.
    /// </param>
    /// <param name="token">Token de sesión a asociar con esta sesión.</param>
    /// <param name="timestamp">
    /// Marca de tiempo de creación de la sesión.
    /// </param>
    public Session(int ttlHours, string token, DateTime timestamp) : base(timestamp)
    {
        TtlHours = ttlHours;
        Token = token;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Session"/>.
    /// </summary>
    public Session()
     : this(default, null!, default)
    {
    }
}
