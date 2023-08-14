#pragma warning disable CS1591

using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public abstract class MiddlewareTestsBase<T> : MiddlewareTestsBase where T : ITransactionMiddleware, new()
{
    protected (TransactionConfiguration testRepo, T perfMon) Build()
    {
        TransactionConfiguration testRepo = new();
        T perfMon = new();
        testRepo.Attach(perfMon);
        return (testRepo, perfMon);
    }
}