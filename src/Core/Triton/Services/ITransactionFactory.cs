namespace TheXDS.Triton.Services;

/// <summary>
/// Defines the contract for types that allows defining
/// the implementation of transaction generation to be used by services.
/// </summary>
public interface ITransactionFactory
{
    /// <summary>
    /// Creates a transaction that allows reading information from the
    /// database.
    /// </summary>
    /// <param name="runner">
    /// Configuration to use when manufacturing a transaction.
    /// </param>
    /// <returns>
    /// A transaction that allows reading information from the database.
    /// </returns>
    ICrudReadTransaction GetReadTransaction(IMiddlewareRunner runner) => GetTransaction(runner);

    /// <summary>
    /// Creates a transaction that allows writing information to the database.
    /// </summary>
    /// <param name="runner">
    /// Configuration to use when manufacturing a transaction.
    /// </param>
    /// <returns>
    /// A transaction that allows writing information to the database.
    /// </returns>
    ICrudWriteTransaction GetWriteTransaction(IMiddlewareRunner runner) => GetTransaction(runner);

    /// <summary>
    /// Creates a transaction that allows reading and writing information in
    /// the database.
    /// </summary>
    /// <param name="runner">
    /// Configuration to use when manufacturing a transaction.
    /// </param>
    /// <returns>
    /// A transaction that allows reading and writing information in the
    /// database.
    /// </returns>
    ICrudReadWriteTransaction GetTransaction(IMiddlewareRunner runner);
}
