#pragma warning disable CS1591

using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests.Services.Services;

public abstract class TransactionTestBase
{
    protected static ICrudReadWriteTransaction GetTransaction(FailureReason reason)
    {
        return GetTransaction(true, reason);
    }

    protected static ICrudReadWriteTransaction GetTransaction()
    {
        return GetTransaction(false, FailureReason.Unknown);
    }

    private static ICrudReadWriteTransaction GetTransaction(bool injectFailure, FailureReason reason)
    {
        return new TritonService(new TestTransFactory() { InjectFailure = injectFailure, FailureReason = reason }).GetTransaction();
    }
}
