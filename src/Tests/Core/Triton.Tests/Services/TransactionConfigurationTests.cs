#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Services;

public class TransactionConfigurationTests
{
    private class TestMiddleware(int expectedPrologOrder, int expectedEpilogOrder, Action<int> onProlog, Action<int> onEpilog) : ITransactionMiddleware
    {
        ServiceResult? ITransactionMiddleware.PrologueAction(CrudAction action, IEnumerable<ChangeTrackerItem>? entities)
        {
            onProlog.Invoke(expectedPrologOrder);
            return null;
        }

        ServiceResult? ITransactionMiddleware.EpilogueAction(CrudAction action, IEnumerable<ChangeTrackerItem>? entities)
        {
            onEpilog.Invoke(expectedEpilogOrder);
            return null;
        }
    }

    [Test]
    public void AttachAt_T_attaches_middlewares_in_correct_location()
    {
        int prologOrder = 1;
        int epilogOrder = 1;
        void CheckPrologOrder(int current)
        {
            Assert.That(current, Is.EqualTo(prologOrder));
            prologOrder++;
        }
        void CheckEpilogOrder(int current)
        {
            Assert.That(current, Is.EqualTo(epilogOrder));
            epilogOrder++;
        }
        TransactionConfiguration t = new();
        Assert.That(t
            .AttachAt(new TestMiddleware(3, 2, CheckPrologOrder, CheckEpilogOrder), ActionPosition.Late, ActionPosition.Default)
            .AttachAt(new TestMiddleware(1, 3, CheckPrologOrder, CheckEpilogOrder), ActionPosition.Early, ActionPosition.Late)
            .AttachAt(new TestMiddleware(2, 1, CheckPrologOrder, CheckEpilogOrder), ActionPosition.Default, ActionPosition.Early), Is.SameAs(t));
        ((IMiddlewareConfigurator)t).GetRunner().RunPrologue(default, null);
        ((IMiddlewareConfigurator)t).GetRunner().RunEpilogue(default, null);
        Assert.That(prologOrder, Is.EqualTo(4));
        Assert.That(epilogOrder, Is.EqualTo(4));
    }

    [Test]
    public void Detach_removes_middleware()
    {
        var middlewareMock = new Mock<ITransactionMiddleware>(MockBehavior.Strict);
        middlewareMock.Setup(p => p.PrologueAction(It.IsAny<CrudAction>(), It.IsAny<IEnumerable<ChangeTrackerItem>?>())).Returns((ServiceResult?)null).Verifiable(Times.Never);
        middlewareMock.Setup(p => p.EpilogueAction(It.IsAny<CrudAction>(), It.IsAny<IEnumerable<ChangeTrackerItem>?>())).Returns((ServiceResult?)null).Verifiable(Times.Never);
        IMiddlewareConfigurator t = new TransactionConfiguration();
        t.Attach(middlewareMock.Object);
        t.Detach(middlewareMock.Object);
        t.GetRunner().RunPrologue(default, []);
        t.GetRunner().RunEpilogue(default, []);
        middlewareMock.Verify();
        middlewareMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Attach_attaches_middleware()
    {
        var middlewareMock = new Mock<ITransactionMiddleware>(MockBehavior.Strict);
        middlewareMock.Setup(p => p.PrologueAction(It.IsAny<CrudAction>(), It.IsAny<IEnumerable<ChangeTrackerItem>?>())).Returns((ServiceResult?)null).Verifiable(Times.Once);
        middlewareMock.Setup(p => p.EpilogueAction(It.IsAny<CrudAction>(), It.IsAny<IEnumerable<ChangeTrackerItem>?>())).Returns((ServiceResult?)null).Verifiable(Times.Once);
        IMiddlewareConfigurator t = new TransactionConfiguration();
        t.Attach(middlewareMock.Object);
        t.GetRunner().RunPrologue(default, []);
        t.GetRunner().RunEpilogue(default, []);
        middlewareMock.Verify();
        middlewareMock.VerifyNoOtherCalls();
    }
}
