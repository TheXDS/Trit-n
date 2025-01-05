#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Diagnostics.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Diagnostics;

public class ReadOnlySimulatorTests
{
    protected static ServiceResult? RunSimulatorFail(IMiddlewareConfigurator testRepo, CrudAction action, IEnumerable<ChangeTrackerItem>? entity)
    {
        if (testRepo.GetRunner().RunPrologue(action, entity) is { } pr) return pr;
        Assert.Fail();
        return testRepo.GetRunner().RunEpilogue(action, entity);
    }

    protected static (ServiceResult?, bool) RunSimulatorPass(IMiddlewareConfigurator testRepo, CrudAction action, IEnumerable<ChangeTrackerItem>? entity)
    {
        if (testRepo.GetRunner().RunPrologue(action, entity) is { } pr) return (pr, false);
        return (testRepo.GetRunner().RunEpilogue(action, entity), true);
    }

    [Test]
    public void Simulator_blocks_action()
    {
        static ServiceResult? CheckBlocked(CrudAction crudAction, IEnumerable<ChangeTrackerItem>? entity)
        {
            Assert.Fail();
            return null;
        }
        var t = new TransactionConfiguration().UseSimulation(false).AddEpilogue(CheckBlocked);
        RunSimulatorFail(t, CrudAction.Create, [new ChangeTrackerItem(null, new User("x", "test"))]);
        RunSimulatorFail(t, CrudAction.Update, [new ChangeTrackerItem(new User("x", "test"), new User("x", "test"))]);
        RunSimulatorFail(t, CrudAction.Delete, [new ChangeTrackerItem(new User("x", "test"), null)]);
        RunSimulatorFail(t, CrudAction.Commit, [new ChangeTrackerItem(null, null)]);
    }

    [Test]
    public void Simulator_allows_Read()
    {
        bool ranEpilog = false;
        ServiceResult? ChkEpilogue(CrudAction crudAction, IEnumerable<ChangeTrackerItem>? entity)
        {
            ranEpilog = true;
            return null;
        }
        var t = new TransactionConfiguration().UseSimulation(false).AddEpilogue(ChkEpilog);
        Assert.That(RunSimulatorPass(t, CrudAction.Read, [new ChangeTrackerItem(null, null)]).Item2);
        Assert.That(ranEpilog);
    }

    [TestCase(CrudAction.Create, false)]
    [TestCase(CrudAction.Update, false)]
    [TestCase(CrudAction.Delete, false)]
    [TestCase(CrudAction.Commit, false)]
    [TestCase(CrudAction.Read, true)]
    public void Simulator_runs_Epilogues(CrudAction action, bool ranTrans)
    {
        bool ranEpilog = false;
        ServiceResult? ChkEpilogue(CrudAction crudAction, IEnumerable<ChangeTrackerItem>? entity)
        {
            ranEpilog = true;
            return null;
        }
        var t = new TransactionConfiguration().UseSimulation().AddEpilogue(ChkEpilog);
        Assert.That(ranTrans, Is.EqualTo(RunSimulatorPass(t, action, [new ChangeTrackerItem(new User("x", "test"), null)]).Item2));
        Assert.That(ranEpilog);
    }
}