#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Services;

public class TritonServiceTests
{
    [Test]
    public void Full_ctor_accepts_IMiddlewareConfigurator_and_ITransactionFactory()
    {
        var cfg = new Mock<IMiddlewareConfigurator>().Object;
        var fac = new Mock<ITransactionFactory>().Object;
        var svc = new TritonService(cfg, fac);
        Assert.That(svc.Configuration, Is.SameAs(cfg));
        Assert.That(svc.Factory, Is.SameAs(fac));
    }
    [Test]
    public void Full_ctor_contract_test()
    {
        Assert.That(() => new TritonService(null!, new Mock<ITransactionFactory>().Object), Throws.ArgumentNullException);
        Assert.That(() => new TritonService(new Mock<IMiddlewareConfigurator>().Object, null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Ctor_with_ITransactionFactory_uses_TritonConfiguration_by_default()
    {
        var svc = new TritonService(new Mock<ITransactionFactory>().Object);
        Assert.That(svc.Configuration, Is.InstanceOf<TransactionConfiguration>());
    }

    [Test]
    public void Service_can_create_read_transactions()
    {
        var readMock = new Mock<ICrudReadTransaction>();
        var runMock = new Mock<IMiddlewareRunner>();
        var cfgMock = new Mock<IMiddlewareConfigurator>();
        var facMock = new Mock<ITransactionFactory>();

        cfgMock.Setup(p => p.GetRunner()).Returns(runMock.Object).Verifiable(Times.Once);
        facMock.Setup(p => p.GetReadTransaction(runMock.Object)).Returns(readMock.Object).Verifiable(Times.Once);

        var svc = new TritonService(cfgMock.Object, facMock.Object);
        Assert.That(svc.GetReadTransaction(), Is.SameAs(readMock.Object));
        cfgMock.Verify();
        facMock.Verify();
    }

    [Test]
    public void Service_can_create_write_transactions()
    {
        var wrtMock = new Mock<ICrudWriteTransaction>();
        var runMock = new Mock<IMiddlewareRunner>();
        var cfgMock = new Mock<IMiddlewareConfigurator>();
        var facMock = new Mock<ITransactionFactory>();

        cfgMock.Setup(p => p.GetRunner()).Returns(runMock.Object).Verifiable(Times.Once);
        facMock.Setup(p => p.GetWriteTransaction(runMock.Object)).Returns(wrtMock.Object).Verifiable(Times.Once);

        var svc = new TritonService(cfgMock.Object, facMock.Object);
        Assert.That(svc.GetWriteTransaction(), Is.SameAs(wrtMock.Object));
        cfgMock.Verify();
        facMock.Verify();
    }

    [Test]
    public void Service_can_create_read_write_transactions()
    {
        var rwtMock = new Mock<ICrudReadWriteTransaction>();
        var runMock = new Mock<IMiddlewareRunner>();
        var cfgMock = new Mock<IMiddlewareConfigurator>();
        var facMock = new Mock<ITransactionFactory>();

        cfgMock.Setup(p => p.GetRunner()).Returns(runMock.Object).Verifiable(Times.Once);
        facMock.Setup(p => p.GetTransaction(runMock.Object)).Returns(rwtMock.Object).Verifiable(Times.Once);

        var svc = new TritonService(cfgMock.Object, facMock.Object);
        Assert.That(svc.GetTransaction(), Is.SameAs(rwtMock.Object));
        cfgMock.Verify();
        facMock.Verify();
    }
}
