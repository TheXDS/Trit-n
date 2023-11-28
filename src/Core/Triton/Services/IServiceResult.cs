namespace TheXDS.Triton.Services;

/// <summary>
/// Representa el resultado devuelto por un servicio al intentar
/// realizar una operación.
/// </summary>
public interface IServiceResult
{
    /// <summary>
    /// Obtiene un mensaje que describe el resultado de la operación.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Obtiene la razón por la cual una operación ha fallado, o
    /// <see langword="null"/> si la operación se completó 
    /// exitosamente.
    /// </summary>
    FailureReason? Reason { get; }

    /// <summary>
    /// Obtiene un valor que indica si la operación ha sido exitosa.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Convierte un resultado simple en uno de un tipo más específico.
    /// </summary>
    /// <typeparam name="TResult">
    /// Tipo de resultado a obtener. Los valores específicos del
    /// resultado tendrán su valor predeterminado.
    /// </typeparam>
    /// <returns>
    /// Un resultado de tipo <typeparamref name="TResult"/>.
    /// </returns>
    TResult CastUp<TResult>() where TResult : ServiceResult, new();

    /// <summary>
    /// Convierte un resultado simple en uno de un tipo más específico.
    /// </summary>
    /// <typeparam name="TResult">Tipo de resultado a obtener.</typeparam>
    /// <param name="result">Resultado a incluir.</param>
    /// <returns>
    /// Un resultado de tipo <see cref="ServiceResult{T}"/> de tipo
    /// <typeparamref name="TResult"/>.
    /// </returns>
    ServiceResult<TResult?> CastUp<TResult>(TResult result);
}