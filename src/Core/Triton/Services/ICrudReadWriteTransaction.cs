namespace TheXDS.Triton.Services;

/// <summary>
/// Defines the contract for types that allows performing read and write
/// operations based on transaction over a database.
/// </summary>
public interface ICrudReadWriteTransaction : ICrudReadTransaction, ICrudWriteTransaction
{
}