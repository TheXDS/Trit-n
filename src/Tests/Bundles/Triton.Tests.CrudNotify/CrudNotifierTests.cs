#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.CrudNotify;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.CrudNotify;

public class CrudNotifierTests
{
    [Test]
    public void Crud_transaction_triggers_notifications_Test()
    {
        var notifierMock = new Mock<ICrudNotifier>();
        notifierMock.Setup(p => p.NotifyPeers(CrudAction.Create, It.IsAny<IEnumerable<ChangeTrackerItem>?>())).Returns((ServiceResult?)null).Verifiable(Times.Once);
        IMiddlewareConfigurator configurator = new TransactionConfiguration();
        configurator.AddNotifyService(notifierMock.Object);
        Assert.That(configurator.GetRunner().RunEpilogue(CrudAction.Create, null), Is.Null);
        notifierMock.Verify();
    }
}