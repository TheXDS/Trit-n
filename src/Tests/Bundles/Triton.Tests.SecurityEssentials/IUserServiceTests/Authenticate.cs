#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Security;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class Authenticate
{
    [Test]
    public async Task Authenticate_with_valid_credentials_succeeds()
    {
        var passwd = PasswordStorage.CreateHash<Pbkdf2Storage>("password".ToSecureString());
        var testCred = new LoginCredential() { Username = "test", Enabled = true, PasswordHash = passwd, Sessions = [] };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);
        
        var writeTransactionMock = new Mock<ICrudWriteTransaction>();
        writeTransactionMock.Setup(p => p.Update(testCred)).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        writeTransactionMock.Setup(p => p.Create(It.IsAny<Session>())).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        writeTransactionMock.Setup(p => p.CommitAsync()).ReturnsAsync(ServiceResult.Ok).Verifiable(Times.Once);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult).Verifiable(Times.Once);
        svcMock.Setup(p => p.GetWriteTransaction()).Returns(writeTransactionMock.Object).Verifiable(Times.Once);

        var t = DateTime.UtcNow;
        var epsilon = TimeSpan.FromSeconds(30);
        var result = await svcMock.Object.Authenticate("test", "password".ToSecureString());

        Assert.That(result.Success, Is.True);
        Assert.That(result.Result, Is.InstanceOf<Session>());
        Assert.That(testCred.Sessions.Count, Is.EqualTo(1));
        Assert.That(testCred.Sessions.Single().Timestamp.IsBetween(t - epsilon, t + epsilon), Is.True);
        Assert.That(testCred.Sessions.Single().Token?.Length, Is.EqualTo(172));
        writeTransactionMock.Verify();
        svcMock.Verify();
    }

    [Test]
    public async Task Authenticate_with_invalid_credentials_returns_forbidden()
    {
        var passwd = PasswordStorage.CreateHash<Pbkdf2Storage>("password".ToSecureString());
        var testCred = new LoginCredential() { Username = "test", Enabled = true, PasswordHash = passwd, Sessions = [] };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult).Verifiable(Times.Once);
        svcMock.Setup(p => p.GetWriteTransaction()).Verifiable(Times.Never);

        var result = await svcMock.Object.Authenticate("test", "invalid".ToSecureString());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Forbidden));
        Assert.That(result.Result, Is.Null);
        Assert.That(testCred.Sessions, Is.Empty);
        svcMock.Verify();
    }

    [Test]
    public async Task Authenticate_with_disabled_credentials_returns_forbidden()
    {
        var passwd = PasswordStorage.CreateHash<Pbkdf2Storage>("password".ToSecureString());
        var testCred = new LoginCredential() { Username = "test", Enabled = false, PasswordHash = passwd, Sessions = [] };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult).Verifiable(Times.Once);
        svcMock.Setup(p => p.GetWriteTransaction()).Verifiable(Times.Never);

        var result = await svcMock.Object.Authenticate("test", "password".ToSecureString());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Forbidden));
        Assert.That(result.Result, Is.Null);
        Assert.That(testCred.Sessions, Is.Empty);
        svcMock.Verify();
    }

    [Test]
    public async Task Authenticate_with_broken_credentials_returns_forbidden()
    {
        var passwd = PasswordStorage.CreateHash<Pbkdf2Storage>("password".ToSecureString());
        var testCred = new LoginCredential() { Username = "test", Enabled = true, PasswordHash = null!, Sessions = [] };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult).Verifiable(Times.Once);
        svcMock.Setup(p => p.GetWriteTransaction()).Verifiable(Times.Never);

        var result = await svcMock.Object.Authenticate("test", "password".ToSecureString());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Forbidden));
        Assert.That(result.Result, Is.Null);
        Assert.That(testCred.Sessions, Is.Empty);
        svcMock.Verify();
    }

    [Test]
    public async Task Authenticate_with_unknown_user_returns_forbidden()
    {
        var getCredResult = ServiceResult.FailWith<ServiceResult<LoginCredential?>>(FailureReason.NotFound);

        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult).Verifiable(Times.Once);
        svcMock.Setup(p => p.GetWriteTransaction()).Verifiable(Times.Never);

        var result = await svcMock.Object.Authenticate("test", "password".ToSecureString());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Forbidden));
        Assert.That(result.Result, Is.Null);
        svcMock.Verify();
    }

    [Test]
    public async Task Authenticate_with_service_failure_returns_failure()
    {
        var getCredResult = new ServiceResult<LoginCredential?>(FailureReason.Tamper);
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult).Verifiable(Times.Once);

        var result = await svcMock.Object.Authenticate("test", "password".ToSecureString());

        Assert.That(result?.Success, Is.False);
        Assert.That(result?.Reason, Is.EqualTo(FailureReason.Tamper));
        svcMock.Verify();
    }
}
