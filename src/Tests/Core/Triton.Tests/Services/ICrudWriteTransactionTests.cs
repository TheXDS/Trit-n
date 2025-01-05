#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Services;

public class ICrudWriteTransactionTests
{
    [Test]
    public void Create_T_calls_non_generic_version()
    {
        var newEntity = new User() { Id = "NewId" };
        var writeMock = new Mock<ICrudWriteTransaction>() { CallBase = true };
        writeMock.Setup(x => x.Create((Model)newEntity)).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        var result = writeMock.Object.Create(newEntity);
        Assert.That(result.Success, Is.True);
        writeMock.Verify();
    }

    [Test]
    public void CreateOrUpdate_T_calls_non_generic_version()
    {
        var newEntity = new User() { Id = "NewId" };
        var writeMock = new Mock<ICrudWriteTransaction>() { CallBase = true };
        writeMock.Setup(x => x.CreateOrUpdate((Model)newEntity)).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        var result = writeMock.Object.CreateOrUpdate(newEntity);
        Assert.That(result.Success, Is.True);
        writeMock.Verify();
    }

    [Test]
    public void Update_T_calls_non_generic_versiony()
    {
        var newEntity = new User() { Id = "NewId" };
        var writeMock = new Mock<ICrudWriteTransaction>() { CallBase = true };
        writeMock.Setup(x => x.Update((Model)newEntity)).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        var result = writeMock.Object.Update(newEntity);
        Assert.That(result.Success, Is.True);
        writeMock.Verify();
    }

    [Test]
    public void Delete_T_calls_non_generic_version()
    {
        var newEntity = new User() { Id = "NewId" };
        var writeMock = new Mock<ICrudWriteTransaction>() { CallBase = true };
        writeMock.Setup(x => x.Delete((Model)newEntity)).Returns(ServiceResult.Ok).Verifiable(Times.Once);
        var result = writeMock.Object.Delete(newEntity);
        Assert.That(result.Success, Is.True);
        writeMock.Verify();
    }
}
