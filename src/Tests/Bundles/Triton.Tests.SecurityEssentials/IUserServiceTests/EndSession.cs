#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.MCART.Helpers;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class EndSession
{
    [Test]
    public async Task EndSession_ends_session()
    {
        var testSession = new Session();
        var writeTransactionMock = new Mock<ICrudWriteTransaction>();
        writeTransactionMock.Setup(p => p.Update(testSession)).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        writeTransactionMock.Setup(p => p.CommitAsync()).ReturnsAsync(ServiceResult.Ok).Verifiable(Times.Once);

        var t = DateTime.UtcNow;
        var epsilon = TimeSpan.FromSeconds(30);
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetWriteTransaction()).Returns(writeTransactionMock.Object).Verifiable(Times.Once);

        Assert.That(await svcMock.Object.EndSession(testSession), Is.EqualTo(ServiceResult.Ok));
        Assert.That(testSession.EndTimestamp.IsBetween(t - epsilon, t + epsilon), Is.True);
        svcMock.Verify();
    }

    [Test]
    public async Task EndSession_with_ended_session_returns_idempotency()
    {
        var testSession = new Session() { EndTimestamp = DateTime.UtcNow };

        var t = DateTime.UtcNow;
        var epsilon = TimeSpan.FromSeconds(30);
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetWriteTransaction()).Verifiable(Times.Never);

        var result = await svcMock.Object.EndSession(testSession);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Idempotency));
        svcMock.Verify();
    }

    [Test]
    public async Task EndSession_with_service_failure_returns_failure()
    {
        var testSession = new Session() { EndTimestamp = null };
        var writeTransactionMock = new Mock<ICrudWriteTransaction>();
        writeTransactionMock.Setup(p => p.Update(testSession)).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        writeTransactionMock.Setup(p => p.CommitAsync()).ReturnsAsync(ServiceResult.FailWith<ServiceResult>(FailureReason.NetworkFailure)).Verifiable(Times.Once);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetWriteTransaction()).Returns(writeTransactionMock.Object).Verifiable(Times.Once);

        var result = await svcMock.Object.EndSession(testSession);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NetworkFailure));
        svcMock.Verify();
    }
}
