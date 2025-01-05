#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Services;

public class TransactionRunnerTests
{
    private class MiddlewareActionCheck(ServiceResult? runResult = null)
    {
        public bool DidRun { get; private set; }
        public CrudAction? CrudAction { get; private set; }
        public IEnumerable<ChangeTrackerItem>? Changes { get; private set; }
        public ServiceResult? OnRun(CrudAction action, IEnumerable<ChangeTrackerItem>? changes)
        {
            DidRun = true;
            CrudAction = action;
            Changes = changes;
            return runResult;
        }
    }

    [Test]
    public void Runner_runs_Prologues()
    {
        var check = new MiddlewareActionCheck();
        ChangeTrackerItem[] changes = [];
        var runner = new TransactionRunner([check.OnRun], []);
        Assert.That(runner.RunPrologue(CrudAction.Update, changes), Is.Null);
        Assert.That(check.DidRun, Is.True);
        Assert.That(check.CrudAction, Is.EqualTo(CrudAction.Update));
        Assert.That(check.Changes, Is.SameAs(changes));
    }

    [Test]
    public void Runner_runs_Epilogues()
    {
        var check = new MiddlewareActionCheck();
        ChangeTrackerItem[] changes = [];
        var runner = new TransactionRunner([], [check.OnRun]);
        Assert.That(runner.RunEpilogue(CrudAction.Update, changes), Is.Null);
        Assert.That(check.DidRun, Is.True);
        Assert.That(check.CrudAction, Is.EqualTo(CrudAction.Update));
        Assert.That(check.Changes, Is.SameAs(changes));
    }

    [Test]
    public void Runner_stops_prologs_on_ServiceResult()
    {
        var check1 = new MiddlewareActionCheck(ServiceResult.Ok);
        var check2 = new MiddlewareActionCheck();
        ChangeTrackerItem[] changes = [];
        var runner = new TransactionRunner([check1.OnRun, check2.OnRun], []);
        Assert.That(runner.RunPrologue(CrudAction.Update, changes), Is.EqualTo(ServiceResult.Ok));
        Assert.That(check1.DidRun, Is.True);
        Assert.That(check1.CrudAction, Is.EqualTo(CrudAction.Update));
        Assert.That(check1.Changes, Is.SameAs(changes));
        Assert.That(check2.DidRun, Is.False);
        Assert.That(check2.CrudAction, Is.Null);
        Assert.That(check2.Changes, Is.Null);
    }

    [Test]
    public void Runner_stops_epilogs_on_ServiceResult()
    {
        var check1 = new MiddlewareActionCheck(ServiceResult.Ok);
        var check2 = new MiddlewareActionCheck();
        ChangeTrackerItem[] changes = [];
        var runner = new TransactionRunner([], [check1.OnRun, check2.OnRun]);
        Assert.That(runner.RunEpilogue(CrudAction.Update, changes), Is.EqualTo(ServiceResult.Ok));
        Assert.That(check1.DidRun, Is.True);
        Assert.That(check1.CrudAction, Is.EqualTo(CrudAction.Update));
        Assert.That(check1.Changes, Is.SameAs(changes));
        Assert.That(check2.DidRun, Is.False);
        Assert.That(check2.CrudAction, Is.Null);
        Assert.That(check2.Changes, Is.Null);
    }
}