#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Services;

public class ICrudReadTransactionTests
{
    [Test]
    public void Read_TModel_TKey_reads_entity()
    {
        var expected = new User { Id = "Test" };
        var readMock = new Mock<ICrudReadTransaction>() { CallBase = true };
        readMock.Setup(p => p.Read<User, string>("Test")).Returns(new ServiceResult<User?>(expected)).Verifiable(Times.Once);
        var result = readMock.Object.Read<User, string>("Test", out var entityRead);
        Assert.That(result.Success, Is.True);
        Assert.That(entityRead, Is.SameAs(expected));
        readMock.Verify();
    }

    [Test]
    public void Read_TModel_TKey_on_failure_returns_failure()
    {
        var readMock = new Mock<ICrudReadTransaction>() { CallBase = true };
        readMock.Setup(p => p.Read<User, string>("Test")).Returns(FailureReason.NotFound).Verifiable(Times.Once);
        var result = readMock.Object.Read<User, string>("Test", out var entityRead);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NotFound));
        readMock.Verify();
    }

    [Test]
    public void Read_TModel_reads_entity()
    {
        var expected = new User { Id = "Test" };
        var readMock = new Mock<ICrudReadTransaction>() { CallBase = true };
        readMock.Setup(p => p.Read<User>("Test")).Returns(new ServiceResult<User?>(expected)).Verifiable(Times.Once);
        var result = readMock.Object.Read<User>("Test", out var entityRead);
        Assert.That(result.Success, Is.True);
        Assert.That(entityRead, Is.SameAs(expected));
        readMock.Verify();
    }

    [Test]
    public void Read_TModel_on_failure_returns_failure()
    {
        var readMock = new Mock<ICrudReadTransaction>() { CallBase = true };
        readMock.Setup(p => p.Read<User>("Test")).Returns(FailureReason.NotFound).Verifiable(Times.Once);
        var result = readMock.Object.Read<User>("Test", out var entityRead);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NotFound));
        readMock.Verify();
    }

    [Test]
    public void Read_reads_entity()
    {
        var expected = new User { Id = "Test" };
        var readMock = new Mock<ICrudReadTransaction>() { CallBase = true };
        readMock.Setup(p => p.Read(typeof(User), "Test")).Returns(new ServiceResult<Model?>(expected)).Verifiable(Times.Once);
        var result = readMock.Object.Read(typeof(User), "Test", out var entityRead);
        Assert.That(result.Success, Is.True);
        Assert.That(entityRead, Is.SameAs(expected));
        readMock.Verify();
    }

    [Test]
    public void Read_on_failure_returns_failure()
    {
        var readMock = new Mock<ICrudReadTransaction>() { CallBase = true };
        readMock.Setup(p => p.Read(typeof(User), "Test")).Returns(FailureReason.NotFound).Verifiable(Times.Once);
        var result = readMock.Object.Read(typeof(User), "Test", out var entityRead);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NotFound));
        readMock.Verify();
    }
}
