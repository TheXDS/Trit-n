#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class ContinueSession
{
    [Test]
    public async Task ContinueSession_returns_session()
    {
        var testSession = new Session()
        {
            Token = "abcd",
            EndTimestamp = null,
            TtlSeconds = 1000,
            Timestamp = DateTime.UtcNow - TimeSpan.FromSeconds(500)
        };
        var readTransactionMock = new Mock<ICrudReadTransaction>();
        readTransactionMock.Setup(p => p.All<Session>()).Returns(new QueryServiceResult<Session>(new[] { testSession }.AsQueryable())).Verifiable(Times.Once);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetReadTransaction()).Returns(readTransactionMock.Object);

        var result = await svcMock.Object.ContinueSession("abcd");

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.SameAs(testSession));
        svcMock.Verify();
    }

    [Test]
    public async Task ContinueSession_with_expired_session_returns_forbidden()
    {
        var testSession = new Session()
        {
            Token = "abcd",
            EndTimestamp = null,
            TtlSeconds = 1000,
            Timestamp = DateTime.UtcNow - TimeSpan.FromSeconds(1500)
        };
        var readTransactionMock = new Mock<ICrudReadTransaction>();
        readTransactionMock.Setup(p => p.All<Session>()).Returns(new QueryServiceResult<Session>(new[] { testSession }.AsQueryable())).Verifiable(Times.Once);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetReadTransaction()).Returns(readTransactionMock.Object);

        var result = await svcMock.Object.ContinueSession("abcd");

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Forbidden));
        svcMock.Verify();
    }

    [Test]
    public async Task ContinueSession_with_unknown_session_returns_forbidden()
    {
        var testSession = new Session()
        {
            Token = "abcd",
            EndTimestamp = null,
            TtlSeconds = 1000,
            Timestamp = DateTime.UtcNow - TimeSpan.FromSeconds(500)
        };
        var readTransactionMock = new Mock<ICrudReadTransaction>();
        readTransactionMock.Setup(p => p.All<Session>()).Returns(new QueryServiceResult<Session>(new[] { testSession }.AsQueryable())).Verifiable(Times.Once);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetReadTransaction()).Returns(readTransactionMock.Object);

        var result = await svcMock.Object.ContinueSession("efgh");

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Forbidden));
        svcMock.Verify();
    }

    [Test]
    public async Task ContinueSession_with_no_sessions_returns_forbidden()
    {
        var readTransactionMock = new Mock<ICrudReadTransaction>();
        readTransactionMock.Setup(p => p.All<Session>()).Returns(new QueryServiceResult<Session>(Array.Empty<Session>().AsQueryable())).Verifiable(Times.Once);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetReadTransaction()).Returns(readTransactionMock.Object);

        var result = await svcMock.Object.ContinueSession("abcd");

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Forbidden));
        svcMock.Verify();
    }

    [Test]
    public async Task ContinueSession_with_service_failure_returns_failure()
    {
        var readTransactionMock = new Mock<ICrudReadTransaction>();
        readTransactionMock.Setup(p => p.All<Session>()).Returns((QueryServiceResult<Session>)FailureReason.NetworkFailure).Verifiable(Times.Once);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetReadTransaction()).Returns(readTransactionMock.Object);

        var result = await svcMock.Object.ContinueSession("abcd");

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NetworkFailure));
        svcMock.Verify();
    }
}