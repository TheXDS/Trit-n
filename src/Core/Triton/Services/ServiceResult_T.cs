namespace TheXDS.Triton.Services;

/// <summary>
/// Representa el resultado de una operación de servicio que incluye un
/// valor devuelto.
/// </summary>
/// <typeparam name="T">
/// Tipo de resultado a devolver.
/// </typeparam>
public class ServiceResult<T> : ServiceResult
{
    /// <summary>
    /// Obtiene el valor a devolver como parte del resultado de la
    /// operación de servicio.
    /// </summary>
    public T ReturnValue { get; } = default!;

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="ServiceResult"/>.
    /// </summary>
    public ServiceResult() : base()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="ServiceResult"/>, indicando un mensaje de estado 
    /// personalizado a mostrar.
    /// </summary>
    /// <param name="message">Mensaje descriptivo del resultado.</param>
    public ServiceResult(string message): base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="ServiceResult"/>.
    /// </summary>
    /// <param name="returnValue">
    /// Objeto relevante retornado por la función.
    /// </param>
    public ServiceResult(T returnValue) : base()
    {
        ReturnValue = returnValue;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="ServiceResult"/>, indicando un mensaje de estado 
    /// personalizado a mostrar.
    /// </summary>
    /// <param name="returnValue">
    /// Objeto relevante retornado por la función.
    /// </param>
    /// <param name="message">Mensaje descriptivo del resultado.</param>
    public ServiceResult(T returnValue, string message) : base(message)
    {
        ReturnValue = returnValue;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="ServiceResult"/>, especificando el motivo por el 
    /// cual la operación ha fallado.
    /// </summary>
    /// <param name="reason">
    /// Motivo por el cual la operación ha fallado.
    /// </param>
    public ServiceResult(in FailureReason reason) : base(reason)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="ServiceResult"/>, especificando el motivo por el 
    /// cual la operación ha fallado, además de un mensaje descriptivo
    /// del resultado.
    /// </summary>
    /// <param name="reason">
    /// Motivo por el cual la operación ha fallado.
    /// </param>
    /// <param name="message">Mensaje descriptivo del resultado.</param>
    public ServiceResult(in FailureReason reason, string message) : base(reason, message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="ServiceResult"/> para una operación fallida,
    /// extrayendo la información relevante a partir de la excepción
    /// especificada.
    /// </summary>
    /// <param name="ex">
    /// Excepción desde la cual obtener el mensaje y un código de error
    /// asociado.
    /// </param>
    public ServiceResult(Exception ex) : base(ex)
    {
    }

    /// <summary>
    /// Convierte implícitamente un valor <typeparamref name="T"/> en
    /// un <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="result">
    /// Resultado de la operación.
    /// </param>
    public static implicit operator T(ServiceResult<T> result) => result.ReturnValue;

    /// <summary>
    /// Convierte implícitamente un <see cref="Exception"/> en un
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="ex">
    /// Excepción desde la cual obtener el mensaje y un código de error
    /// asociado.
    /// </param>
    public static implicit operator ServiceResult<T>(Exception ex) => new(ex);

    /// <summary>
    /// Convierte implícitamente un <see cref="string"/> en un
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="message">Mensaje descriptivo del resultado.</param>
    public static implicit operator ServiceResult<T>(string message) => new(message);

    /// <summary>
    /// Convierte implícitamente un <see cref="FailureReason"/> en un
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="reason">
    /// Motivo por el cual la operación ha fallado.
    /// </param>
    public static implicit operator ServiceResult<T>(in FailureReason reason) => new(reason);

    /// <summary>
    /// Convierte implícitamente un <typeparamref name="T"/> en un
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="value">
    /// Valor a partir del cual generar un nuevo <see cref="ServiceResult{T}"/>.
    /// </param>
    public static implicit operator ServiceResult<T>(T value) => new(value);
}