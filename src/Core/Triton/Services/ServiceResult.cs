using System.Diagnostics.CodeAnalysis;
using St = TheXDS.Triton.Resources.Strings.Common;

namespace TheXDS.Triton.Services;

/// <summary>
/// Represents the result returned by a service when attempting to
/// perform an operation.
/// </summary>
public class ServiceResult : IEquatable<ServiceResult>, IEquatable<Exception>
{
    private static string MessageFrom(in FailureReason reason)
    {
        return reason switch
        {
            FailureReason.Unknown => St.FailureUnknown,
            FailureReason.Tamper => St.Tamper,
            FailureReason.Forbidden => St.FailureForbidden,
            FailureReason.ServiceFailure => St.ServiceFailure,
            FailureReason.NetworkFailure => St.NetworkFailure,
            FailureReason.DbFailure => St.DbFailure,
            FailureReason.ValidationError => St.ValidationError,
            FailureReason.ConcurrencyFailure => St.ConcurrencyFailure,
            FailureReason.NotFound => St.EntityNotFound,
            FailureReason.BadQuery => St.BadQuery,
            FailureReason.QueryOverLimit => St.QueryOverLimit,
            FailureReason.Idempotency => St.Idempotency,
            _ => $"0x{reason.ToString("X").PadLeft(8, '0')}",
        };
    }

    /// <summary>
    /// Gets a simple result indicating that the operation was successful.
    /// </summary>
    public static ServiceResult Ok { get; } = SucceedWith<ServiceResult>(null);

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Reason))]
    public bool Success { get; private set; }

    /// <summary>
    /// Gets a message describing the result of the operation.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Gets the reason why an operation failed, or null if the
    /// operation was successful.
    /// </summary>
    public FailureReason? Reason { get; private set; } = null;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// indicating that the operation was successful.
    /// </summary>
    public ServiceResult() : this(true, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// indicating that an operation was successful, and includes a custom
    /// status message to display.
    /// </summary>
    /// <param name="message">A descriptive message for the result.</param>
    public ServiceResult(string? message) : this(true, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// including a custom status message to display.
    /// </summary>
    /// <param name="success">
    /// Indicates whether the operation was completed successfully.
    /// </param>
    /// <param name="message">A descriptive message for the result.</param>
    public ServiceResult(bool success, string? message)
    {
        Success = success;
        Message = message ?? (success ? St.OperationCompletedSuccessfully : St.FailureUnknown);
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// specifying the reason why the operation failed.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation failed.
    /// </param>
    public ServiceResult(in FailureReason reason)
    {
        Success = false;
        Reason = reason;
        Message = MessageFrom(reason);
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/>,
    /// specifying the reason why the operation failed, and including a
    /// descriptive message for the result.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation failed.
    /// </param>
    /// <param name="message">A descriptive message for the result.</param>
    public ServiceResult(in FailureReason reason, string message)
    {
        Success = false;
        Reason = reason;
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ServiceResult"/> for
    /// a failed operation,
    /// extracting relevant information from the specified exception.
    /// </summary>
    /// <param name="ex">
    /// The exception from which to obtain the message and error code
    /// associated with it.
    /// </param>
    public ServiceResult(Exception ex)
    {
        Success = false;
        Message = ex.Message;
        Reason = (FailureReason)ex.HResult;
    }

    /// <summary>
    /// Compares equality between this instance and another instance of the
    /// class <see cref="ServiceResult"/>.
    /// </summary>
    /// <param name="other">
    /// The instance to compare with this instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if both instances are considered equal or
    /// equivalent, <see langword="false"/> otherwise.
    /// </returns>
    public bool Equals([AllowNull] ServiceResult other)
    {
        if (other is null) return false;

        return Success == other.Success && other.Reason == FailureReason.Unknown
            ? Message == other.Message
            : Reason == other.Reason;
    }

    /// <summary>
    /// Compares equality between this instance of the class
    /// <see cref="ServiceResult"/> and an exception object.
    /// </summary>
    /// <param name="other">
    /// The instance to compare with this instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if both instances are considered equal or
    /// equivalent, <see langword="false"/> otherwise.
    /// </returns>
    public bool Equals([AllowNull] Exception other)
    {
        return (int?)Reason == other?.HResult;
    }

    /// <summary>
    /// Converts a simple result into a more specific type.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result to obtain. The specific values of the result have
    /// their default values.
    /// </typeparam>
    /// <returns>
    /// A result of the type <typeparamref name="TResult"/>.</returns>
    public TResult CastUp<TResult>() where TResult : ServiceResult, new()
    {
        return new TResult()
        {
            Success = Success,
            Reason = Reason,
            Message = Message
        };
    }

    /// <summary>
    /// Converts a simple result into a more specific type.
    /// </summary>
    /// <typeparam name="TResult">The type of result to obtain.</typeparam>
    /// <param name="result">The result to include.</param>
    /// <returns>
    /// A result of type <see cref="ServiceResult{T}"/> of type
    /// <typeparamref name="TResult"/>.
    /// </returns>
    public ServiceResult<TResult?> CastUp<TResult>(TResult result)
    {
        return new(result)
        {
            Success = Success,
            Reason = Reason,
            Message = Message
        };
    }

    /// <inheritdoc/>
    public override bool Equals([AllowNull] object obj)
    {
        return obj switch
        {
            ServiceResult s => Equals(s),
            Exception ex => Equals(ex),
            bool b => Success == b,
            _ => false
        };
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Success, Message, Reason);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Message;
    }

    /// <summary>
    /// Gets a failed <typeparamref name="TServiceResult"/> from the produced
    /// exception.
    /// </summary>
    /// <typeparam name="TServiceResult">
    /// The type of <see cref="ServiceResult"/> to generate.
    /// </typeparam>
    /// <param name="ex">
    /// The produced exception.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TServiceResult"/> that represents a failed
    /// operation.
    /// </returns>
    protected static TServiceResult FailWith<TServiceResult>(Exception ex) where TServiceResult : ServiceResult, new()
    {
        return new TServiceResult()
        {
            Success = false,
            Message = ex.Message,
            Reason = (FailureReason)ex.HResult
        };
    }

    /// <summary>
    /// Gets a failed <typeparamref name="TServiceResult"/> from the reported
    /// failure.
    /// </summary>
    /// <typeparam name="TServiceResult">
    /// The type of <see cref="ServiceResult"/> to generate.
    /// </typeparam>
    /// <param name="reason">
    /// The failure that occurred during the operation.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TServiceResult"/> that represents a failed
    /// operation.
    /// </returns>
    protected static TServiceResult FailWith<TServiceResult>(in FailureReason reason) where TServiceResult : ServiceResult, new()
    {
        return new TServiceResult()
        {
            Success = false,
            Reason = reason,
            Message = MessageFrom(reason)
        };
    }

    /// <summary>
    /// Gets a failed <typeparamref name="TServiceResult"/> from the error
    /// message.
    /// </summary>
    /// <typeparam name="TServiceResult">
    /// The type of <see cref="ServiceResult"/> to generate.
    /// </typeparam>
    /// <param name="message">
    /// The error message produced during the operation.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TServiceResult"/> that represents a failed
    /// operation.
    /// </returns>
    protected static TServiceResult FailWith<TServiceResult>(string? message) where TServiceResult : ServiceResult, new()
    {
        return new TServiceResult()
        {
            Success = false,
            Reason = FailureReason.Unknown,
            Message = message ?? St.FailureUnknown
        };
    }

    /// <summary>
    /// Gets a successful <typeparamref name="TServiceResult"/> from the
    /// message.
    /// </summary>
    /// <typeparam name="TServiceResult">
    /// The type of <see cref="ServiceResult"/> to generate.
    /// </typeparam>
    /// <param name="message">
    /// The message produced by the operation.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TServiceResult"/> that represents a successful
    /// operation with an output message.
    /// </returns>
    protected static TServiceResult SucceedWith<TServiceResult>(string? message) where TServiceResult : ServiceResult, new()
    {
        return new TServiceResult()
        {
            Success = true,
            Reason = null,
            Message = message ?? St.OperationCompletedSuccessfully
        };
    }

    /// <summary>
    /// Implicitly converts an <see cref="Exception"/> to a
    /// <see cref="ServiceResult"/>.
    /// </summary>
    /// <param name="ex">
    /// The exception from which to obtain the message and associated error
    /// code.
    /// </param>
    public static implicit operator ServiceResult(Exception ex) => FailWith<ServiceResult>(ex);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a
    /// <see cref="ServiceResult"/>.
    /// </summary>
    /// <param name="message">A descriptive message of the result.</param>
    public static implicit operator ServiceResult(string message) => FailWith<ServiceResult>(message);

    /// <summary>
    /// Implicitly converts a <see cref="FailureReason"/> to a
    /// <see cref="ServiceResult"/>.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation failed.
    /// </param>
    public static implicit operator ServiceResult(in FailureReason reason) => FailWith<ServiceResult>(reason);

    /// <summary>
    /// Implicitly converts a <see cref="bool"/> to a
    /// <see cref="ServiceResult"/>.
    /// </summary>
    /// <param name="success">
    /// A value indicating whether the operation was successful or not.
    /// </param>
    public static implicit operator ServiceResult(in bool success) => success ? Ok : FailWith<ServiceResult>(FailureReason.Unknown);
}