namespace TheXDS.Triton.Services;

/// <summary>
/// Defines a set of members to be implemented by a service that provides
/// extended functionality for generating read/write transactions.
/// </summary>
public interface ITritonService
{
    /// <summary>
    /// Gets a transaction that allows reading information from the database.
    /// </summary>
    /// <returns>
    /// A transaction that allows reading information from the database.
    /// </returns>
    ICrudReadTransaction GetReadTransaction() => GetTransaction();

    /// <summary>
    /// Gets a transaction that allows writing information to the database.
    /// </summary>
    /// <returns>
    /// A transaction that allows writing information to the database.
    /// </returns>
    ICrudWriteTransaction GetWriteTransaction() => GetTransaction();

    /// <summary>
    /// Gets a transaction that allows reading and writing information in the
    /// database.
    /// </summary>
    /// <returns>
    /// A transaction that allows reading and writing information in the
    /// database.
    /// </returns>
    ICrudReadWriteTransaction GetTransaction();
}