using System.Collections;
using System.Linq.Expressions;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Services;

/// <summary>
/// Represents the result of a service operation that includes an
/// <see cref="IQueryable{T}"/> query.
/// </summary>
/// <typeparam name="T">
/// The type of entities to return.
/// </typeparam>
public class QueryServiceResult<T> : ServiceResult, IQueryable<T> where T : Model
{
    private readonly IQueryable<T>? _result;

    /// <inheritdoc/>
    public Type ElementType => _result?.ElementType ?? throw new InvalidOperationException();

    /// <inheritdoc/>
    public Expression Expression => _result?.Expression ?? throw new InvalidOperationException();

    /// <inheritdoc/>
    public IQueryProvider Provider => _result?.Provider ?? throw new InvalidOperationException();

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _result?.GetEnumerator() ?? throw new InvalidOperationException();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _result?.GetEnumerator() ?? throw new InvalidOperationException();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryServiceResult{T}"/>
    /// class without results.
    /// </summary>
    public QueryServiceResult() : base(FailureReason.NotFound)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryServiceResult{T}"/>
    /// class that indicates success, specifying the query result from the
    /// operation.
    /// </summary>
    /// <param name="query">
    /// <see cref="IQueryable{T}"/> with the data query result.
    /// </param>
    public QueryServiceResult(IQueryable<T> query)
    {
        _result = query;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryServiceResult{T}"/>
    /// class, specifying the reason why the operation failed.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation failed.
    /// </param>
    public QueryServiceResult(in FailureReason reason) : base(reason)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryServiceResult{T}"/>
    /// class, specifying the reason why the operation failed, as well as a
    /// descriptive message for the result.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation failed.
    /// </param>
    /// <param name="message">A descriptive message for the result.</param>
    public QueryServiceResult(in FailureReason reason, string message) : base(reason, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryServiceResult{T}"/>
    /// class for a failed operation, extracting relevant information from the
    /// specified exception.
    /// </summary>
    /// <param name="ex">
    /// The exception from which to obtain the error message and associated
    /// error code.
    /// </param>
    public QueryServiceResult(Exception ex) : base(ex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryServiceResult{T}"/>
    /// class, indicating a custom status message to display.
    /// </summary>
    /// <param name="message">A descriptive message for the result.</param>
    public QueryServiceResult(string message) : base(FailureReason.Unknown, message)
    {
    }

    /// <summary>
    /// Implicitly converts an <see cref="Exception"/> to a
    /// <see cref="QueryServiceResult{T}"/>.
    /// </summary>
    /// <param name="ex">
    /// The exception from which to obtain the error message and associated
    /// error code.
    /// </param>
    public static implicit operator QueryServiceResult<T>(Exception ex) => new(ex);

    /// <summary>
    /// Implicitly converts a <see cref="string"/> to a
    /// <see cref="QueryServiceResult{T}"/>.
    /// </summary>
    /// <param name="message">A descriptive message for the result.</param>
    public static implicit operator QueryServiceResult<T>(string message) => new(message);

    /// <summary>
    /// Implicitly converts a <see cref="FailureReason"/> to a
    /// <see cref="QueryServiceResult{T}"/>.
    /// </summary>
    /// <param name="reason">
    /// The reason why the operation failed.
    /// </param>
    public static implicit operator QueryServiceResult<T>(in FailureReason reason) => new(reason);

    /// <summary>
    /// Implicitly converts a <see cref="QueryServiceResult{T}"/> to a
    /// <see cref="bool"/>.
    /// </summary>
    /// <param name="result">
    /// The object to convert.
    /// </param>
    public static implicit operator bool(in QueryServiceResult<T> result) => result.Success;
}