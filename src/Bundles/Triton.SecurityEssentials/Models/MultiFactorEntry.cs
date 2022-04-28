namespace TheXDS.Triton.Models;
using System;
using TheXDS.Triton.Models.Base;

/// <summary>
/// Representa una entrada de datos de autenticación en dos factores.
/// </summary>
public class MultiFactorEntry : Model<Guid>
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="MultiFactorEntry"/>.
    /// </summary>
    public MultiFactorEntry()
        : this(Guid.Empty, null!)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="MultiFactorEntry"/>.
    /// </summary>
    /// <param name="mfaPreprocessor">
    /// <see cref="Guid"/> utilizado para identificar el procesador de MFA a
    /// utilizar para verificar esta entidad.
    /// </param>
    /// <param name="data">
    /// Blob binario de datos personalizados a utilizar por el procesador de
    /// autenticación en dos factores.
    /// </param>
    public MultiFactorEntry(Guid mfaPreprocessor, byte[] data)
    {
        MfaProcessor = mfaPreprocessor;
        Data = data;
    }

    /// <summary>
    /// Obtiene o establece al usuario que posee esta entrada de autenticación
    /// en dos factores.
    /// </summary>
    public LoginCredential User { get; set; } = null!;

    ///
    /// <summary>
    /// Obtiene o establece el <see cref="Guid"/> utilizado para identificar el
    /// procesador de MFA a utilizar para verificar esta entidad.
    /// </summary>
    public Guid MfaProcessor { get; set; }

    /// <summary>
    /// Obtiene o establece un blob binario de datos personalizados a utilizar
    /// por el procesador de autenticación en dos factores.
    /// </summary>
    public byte[] Data { get; set; }
}
