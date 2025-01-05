#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Security;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Models;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class VerifyPassword
{
    [Test]
    public async Task VerifyPassword_with_valid_user_succeeds()
    {
        var passwd = PasswordStorage.CreateHash<Pbkdf2Storage>("password".ToSecureString());
        var testCred = new LoginCredential() { Username = "test", Enabled = true, PasswordHash = passwd };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);
        var svcMock = new Mock<IUserService>() { CallBase = true };

        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult);
        var result = await svcMock.Object.VerifyPassword("test", "password".ToSecureString());

        Assert.That(result?.Success, Is.True);
        Assert.That(result?.Result, Is.Not.Null);
        Assert.That(result?.Result?.Valid, Is.True);
        Assert.That(result?.Result?.LoginCredential, Is.SameAs(testCred));
    }

    [Test]
    public async Task VerifyPassword_with_unknown_user_returns_invalid_result()
    {
        var getCredResult = new ServiceResult<LoginCredential?>(FailureReason.NotFound);
        var svcMock = new Mock<IUserService>() { CallBase = true };

        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult);
        var result = await svcMock.Object.VerifyPassword("test", "password".ToSecureString());

        Assert.That(result?.Success, Is.True);
        Assert.That(result?.Result, Is.Not.Null);
        Assert.That(result?.Result?.Valid, Is.False);
        Assert.That(result?.Result?.LoginCredential, Is.Null);
    }

    [Test]
    public async Task VerifyPassword_with_invalid_password_returns_invalid_result()
    {
        var passwd = PasswordStorage.CreateHash<Pbkdf2Storage>("password".ToSecureString());
        var testCred = new LoginCredential() { Username = "test", Enabled = true, PasswordHash = passwd };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);
        var svcMock = new Mock<IUserService>() { CallBase = true };

        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult);
        var result = await svcMock.Object.VerifyPassword("test", "incorrect".ToSecureString());

        Assert.That(result?.Success, Is.True);
        Assert.That(result?.Result, Is.Not.Null);
        Assert.That(result?.Result?.Valid, Is.False);
        Assert.That(result?.Result?.LoginCredential, Is.Null);
    }

    [Test]
    public async Task VerifyPassword_with_broken_credetial_returns_invalid_result()
    {
        var testCred = new LoginCredential() { Username = "test", Enabled = true, PasswordHash = null! };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);
        var svcMock = new Mock<IUserService>() { CallBase = true };

        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult);
        var result = await svcMock.Object.VerifyPassword("test", "password".ToSecureString());

        Assert.That(result?.Success, Is.True);
        Assert.That(result?.Result, Is.Not.Null);
        Assert.That(result?.Result?.Valid, Is.False);
        Assert.That(result?.Result?.LoginCredential, Is.Null);
    }

    [Test]
    public async Task VerifyPassword_with_disabled_credetial_returns_invalid_result()
    {
        var passwd = PasswordStorage.CreateHash<Pbkdf2Storage>("password".ToSecureString());
        var testCred = new LoginCredential() { Username = "test", Enabled = false, PasswordHash = passwd };
        var getCredResult = new ServiceResult<LoginCredential?>(testCred);
        var svcMock = new Mock<IUserService>() { CallBase = true };

        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult);
        var result = await svcMock.Object.VerifyPassword("test", "password".ToSecureString());

        Assert.That(result?.Success, Is.True);
        Assert.That(result?.Result, Is.Not.Null);
        Assert.That(result?.Result?.Valid, Is.False);
        Assert.That(result?.Result?.LoginCredential, Is.Null);
    }

    [Test]
    public async Task VerifyPassword_with_service_failure_returns_failure()
    {
        var getCredResult = new ServiceResult<LoginCredential?>(FailureReason.Tamper);
        var svcMock = new Mock<IUserService>() { CallBase = true };

        svcMock.Setup(p => p.GetCredential("test")).ReturnsAsync(getCredResult).Verifiable(Times.Once);
        var result = await svcMock.Object.VerifyPassword("test", "password".ToSecureString());

        Assert.That(result?.Success, Is.False);
        Assert.That(result?.Reason, Is.EqualTo(FailureReason.Tamper));
        svcMock.Verify();
    }
}
