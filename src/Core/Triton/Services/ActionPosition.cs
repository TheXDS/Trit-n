namespace TheXDS.Triton.Services;

/// <summary>
/// Enumeración que describe la posición en la cual se agregará una
/// acción de Middleware de Crud.
/// </summary>
public enum ActionPosition : byte
{
    /// <summary>
    /// Posición predeterminada.
    /// </summary>
    Default,
    /// <summary>
    /// Asegurar primera acción.
    /// </summary>
    Early,
    /// <summary>
    /// Asegurar última acción.
    /// </summary>
    Late
}
