#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using System.Security;
using TheXDS.MCART.Security;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class HashPasswordAsync
{
    [Test]
    public async Task HashPasswordAsync_without_generics_defaults_to_pbkdf2()
    {
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.HashPasswordAsync<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .ReturnsAsync([1, 2, 3])
            .Verifiable(Times.Once);

        Assert.That(await svcMock.Object.HashPasswordAsync("test".ToSecureString()), Is.EquivalentTo((byte[])[1, 2, 3]));
        svcMock.Verify();
    }

    [Test]
    public async Task HashPasswordAsync_creates_hash()
    {
        var svcMock = new Mock<IUserService>() { CallBase = true };
        svcMock.Setup(p => p.HashPasswordAsync<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .ReturnsAsync([1, 2, 3])
            .Verifiable(Times.Once);

        Assert.That(await svcMock.Object.HashPasswordAsync<Pbkdf2Storage>("test".ToSecureString()), Is.EquivalentTo((byte[])[1, 2, 3]));
        svcMock.Verify();
    }
}
