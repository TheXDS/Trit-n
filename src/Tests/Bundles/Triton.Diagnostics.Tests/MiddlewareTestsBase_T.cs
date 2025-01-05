#pragma warning disable CS1591

using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public abstract class MiddlewareTestsBase<T> where T : ITransactionMiddleware, new()
{
    protected (IMiddlewareRunner runner, T perfMon) Build()
    {
        return (((IMiddlewareConfigurator)new TransactionConfiguration()).Attach<T>(out var perfMon).GetRunner(), perfMon);
    }

    protected void RunCrudAction(IMiddlewareRunner runner, CrudAction action = CrudAction.Commit)
    {
        runner.RunPrologue(action, null);
        runner.RunEpilogue(action, null);
    }
}