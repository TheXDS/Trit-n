#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Diagnostics;

public class ReadOnlySimulatorTests : MiddlewareTestsBase
{
    protected static ServiceResult? RunSimulatorFail(IMiddlewareConfigurator testRepo, CrudAction action, Model? entity)
    {
        if (testRepo.GetRunner().RunProlog(action, entity) is { } pr) return pr;
        Assert.Fail();
        return testRepo.GetRunner().RunEpilog(action, entity);
    }

    protected static (ServiceResult?, bool) RunSimulatorPass(IMiddlewareConfigurator testRepo, CrudAction action, Model? entity)
    {
        if (testRepo.GetRunner().RunProlog(action, entity) is { } pr) return (pr, false);
        return (testRepo.GetRunner().RunEpilog(action, entity), true);
    }

    [Test]
    public void Simulator_blocks_action()
    {
        static ServiceResult? CheckBlocked(CrudAction crudAction, Model? entity)
        {
            Assert.Fail();
            return null;
        }
        var t = new TransactionConfiguration().UseSimulation(false).AddEpilog(CheckBlocked);
        RunSimulatorFail(t, CrudAction.Create, new User("x", "test"));
        RunSimulatorFail(t, CrudAction.Update, new User("x", "test"));
        RunSimulatorFail(t, CrudAction.Delete, new User("x", "test"));
        RunSimulatorFail(t, CrudAction.Commit, new User("x", "test"));
    }

    [Test]
    public void Simulator_allows_Read()
    {
        bool ranEpilog = false;
        ServiceResult? ChkEpilog(CrudAction crudAction, Model? entity)
        {
            ranEpilog = true;
            return null;
        }
        var t = new TransactionConfiguration().UseSimulation(false).AddEpilog(ChkEpilog);
        Assert.IsTrue(RunSimulatorPass(t, CrudAction.Read, new User("x", "test")).Item2);
        Assert.True(ranEpilog);
    }

    [TestCase(CrudAction.Create, false)]
    [TestCase(CrudAction.Update, false)]
    [TestCase(CrudAction.Delete, false)]
    [TestCase(CrudAction.Commit, false)]
    [TestCase(CrudAction.Read, true)]
    public void Simulator_runs_epilogs(CrudAction action, bool ranTrans)
    {
        bool ranEpilog = false;
        ServiceResult? ChkEpilog(CrudAction crudAction, Model? entity)
        {
            ranEpilog = true;
            return null;
        }
        var t = new TransactionConfiguration().UseSimulation().AddEpilog(ChkEpilog);
        Assert.AreEqual(ranTrans, RunSimulatorPass(t, action, new User("x", "test")).Item2);
        Assert.True(ranEpilog);
    }
}