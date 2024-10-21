#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using System.Security;
using TheXDS.MCART.Security;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class HashPassword
{
    [Test]
    public void HashPassword_without_generics_defaults_to_pbkdf2()
    {
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.HashPassword<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .Returns([1, 2, 3])
            .Verifiable(Times.Once);

        Assert.That(svcMock.Object.HashPassword("test".ToSecureString()), Is.EquivalentTo((byte[])[1, 2, 3]));
        svcMock.Verify();
    }

    [Test]
    public void HashPassword_generates_hash()
    {
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.HashPassword<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .Returns([1, 2, 3])
            .Verifiable(Times.Once);

        Assert.That(svcMock.Object.HashPassword<Pbkdf2Storage>("test".ToSecureString()), Is.EquivalentTo((byte[])[1, 2, 3]));
        svcMock.Verify();
    }
}
