namespace TheXDS.Triton.Services;

/// <summary>
/// Enumeration that describes the position in which a Crud Middleware action
/// will be added.
/// </summary>
public enum ActionPosition : byte
{
    /// <summary>
    /// Default position.
    /// </summary>
    Default,
    /// <summary>
    /// Ensure first action.
    /// </summary>
    Early,
    /// <summary>
    /// Ensure last action.
    /// </summary>
    Late
}