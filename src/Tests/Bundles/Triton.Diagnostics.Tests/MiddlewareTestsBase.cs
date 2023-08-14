#pragma warning disable CS1591

using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public abstract class MiddlewareTestsBase
{
    protected static Task<ServiceResult?> Run(IMiddlewareConfigurator testRepo, CrudAction action, int delayMs = 100)
    {
        return Run(testRepo, action, null, delayMs);
    }

    protected static async Task<ServiceResult?> Run(IMiddlewareConfigurator testRepo, CrudAction action, IEnumerable<Model>? entity, int delayMs = 100)
    {
        if (testRepo.GetRunner().RunProlog(action, entity) is { } pr) return pr;
        await Task.Delay(delayMs);
        return testRepo.GetRunner().RunEpilog(action, entity);
    }
}
