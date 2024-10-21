#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class GetCredentials
{
    [Test]
    public async Task GetCredential_returns_credential_from_own_service()
    {
        var testCred = new LoginCredential() { Username = "test" };
        var searchResult = new ServiceResult<LoginCredential[]?>([testCred]);
        var transactionMock = new Mock<ICrudReadTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>())).ReturnsAsync(searchResult);
        svcMock.Setup(p => p.GetReadTransaction()).Returns(transactionMock.Object);

        var result = await svcMock.Object.GetCredential("test");

        Assert.That(result?.Success, Is.True);
        Assert.That(result?.Result, Is.SameAs(testCred));
    }

    [Test]
    public async Task GetCredential_fails_with_not_found_service_error()
    {
        var searchResult = new ServiceResult<LoginCredential[]?>([]);
        var transactionMock = new Mock<ICrudReadTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>())).ReturnsAsync(searchResult);
        svcMock.Setup(p => p.GetReadTransaction()).Returns(transactionMock.Object);

        var result = await svcMock.Object.GetCredential("test");

        Assert.That(result?.Success, Is.False);
        Assert.That(result?.Reason, Is.EqualTo(FailureReason.NotFound));
    }

    [Test]
    public async Task GetCredential_returns_underlying_service_failures()
    {
        var searchResult = new ServiceResult<LoginCredential[]?>(FailureReason.Tamper);
        var transactionMock = new Mock<ICrudReadTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>())).ReturnsAsync(searchResult);
        svcMock.Setup(p => p.GetReadTransaction()).Returns(transactionMock.Object);

        var result = await svcMock.Object.GetCredential("test");

        Assert.That(result?.Success, Is.False);
        Assert.That(result?.Reason, Is.EqualTo(FailureReason.Tamper));
    }
}
