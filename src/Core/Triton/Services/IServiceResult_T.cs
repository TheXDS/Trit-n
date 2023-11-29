namespace TheXDS.Triton.Services;

/// <summary>
/// Representa el resultado de una operación de servicio que incluye un
/// valor devuelto.
/// </summary>
/// <typeparam name="T">
/// Tipo de resultado a devolver.
/// </typeparam>
public interface IServiceResult<out T> : IServiceResult
{
    /// <summary>
    /// Obtiene el valor a devolver como parte del resultado de la
    /// operación de servicio.
    /// </summary>
    T Result { get; }
}
