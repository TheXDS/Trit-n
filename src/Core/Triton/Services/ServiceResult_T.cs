namespace TheXDS.Triton.Services;

/// <summary>
/// Represents the result of a service operation that includes a returned value.
/// </summary>
/// <typeparam name="T">
/// The type of the result to return.
/// </typeparam>
public class ServiceResult<T> : ServiceResult
{
    /// <summary>
    /// Gets the value to be returned as part of the service operation result.
    /// </summary>
    public T Result { get; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceResult"/> class.
    /// </summary>
    public ServiceResult() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceResult"/> class,
    /// indicating a custom status message to display.
    /// </summary>
    /// <param name="message">A descriptive message for the result.</param>
    public ServiceResult(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>.
    /// </summary>
    /// <param name="result">
    /// The relevant object returned by the function.
    /// </param>
    public ServiceResult(T result) : base()
    {
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// indicating a custom status message to display.
    /// </summary>
    /// <param name="result">
    /// The relevant object returned by the function.
    /// </param>
    /// <param name="message">A descriptive message for the result.</param>
    public ServiceResult(T result, string message) : base(message)
    {
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// specifying the reason why the operation has failed.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation has failed.
    /// </param>
    public ServiceResult(in FailureReason reason) : base(reason)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// specifying the reason why the operation has failed, in addition to
    /// a descriptive message for the result.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation has failed.
    /// </param>
    /// <param name="message">A descriptive message for the result.</param>
    public ServiceResult(in FailureReason reason, string message) : base(reason, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>
    /// for an operation that failed, extracting relevant information from
    /// the specified exception.
    /// </summary>
    /// <param name="ex">
    /// The exception from which to obtain the message and error code.
    /// </param>
    public ServiceResult(Exception ex) : base(ex)
    {
    }

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="result">
    /// The result of the operation.
    /// </param>
    public static implicit operator T(ServiceResult<T> result) => result.Result;

    /// <summary>
    /// Implicitly converts an <see cref="Exception"/> to a
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="ex">
    /// The exception from which to obtain the message and error code.
    /// </param>
    public static implicit operator ServiceResult<T>(Exception ex) => FailWith<ServiceResult<T>>(ex);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="message">A descriptive message for the result.</param>
    public static implicit operator ServiceResult<T>(string message) => FailWith<ServiceResult<T>>(message);

    /// <summary>
    /// Implicitly converts a <see cref="FailureReason"/> to a
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation failed.
    /// </param>
    public static implicit operator ServiceResult<T>(in FailureReason reason) => FailWith<ServiceResult<T>>(reason);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a
    /// <see cref="ServiceResult{T}"/>.
    /// </summary>
    /// <param name="value">
    /// The value from which to generate a new <see cref="ServiceResult{T}"/>.
    /// </param>
    public static implicit operator ServiceResult<T>(T value) => new(value);
}