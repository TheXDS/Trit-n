#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Services;

public class IMiddlewareConfiguratorTests
{
    [ExcludeFromCodeCoverage]
    private class DummyTransactionMiddleware : ITransactionMiddleware { }

    [Test]
    public void Attach_T_attaches_middleware_at_default_location()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        configuratorMock.Setup(p => p.AttachAt<DummyTransactionMiddleware>(ActionPosition.Default))
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.Attach<DummyTransactionMiddleware>();
        configuratorMock.Verify();
    }

    [Test]
    public void Attach_T_with_out_T_attaches_middleware_at_default_location()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        var expectedMiddlewareInstance = new DummyTransactionMiddleware();
        configuratorMock.Setup(p => p.AttachAt<DummyTransactionMiddleware>(out expectedMiddlewareInstance, ActionPosition.Default))
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.Attach<DummyTransactionMiddleware>(out var actualMiddlewareInstance);
        Assert.That(actualMiddlewareInstance, Is.SameAs(expectedMiddlewareInstance));
        configuratorMock.Verify();
    }

    [Test]
    public void Attach_T_with_T_attaches_middleware_at_default_location()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        var middlewareInstance = new DummyTransactionMiddleware();
        configuratorMock.Setup(p => p.AttachAt(middlewareInstance, ActionPosition.Default))
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.Attach(middlewareInstance);
        configuratorMock.Verify();
    }

    [Test]
    public void AttachAt_T_with_ActionPosition_ActionPosition_creates_new_middleware_instance()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        DummyTransactionMiddleware? createdInstance = null;
        void OnAttach(DummyTransactionMiddleware instance, in ActionPosition a, in ActionPosition b) => createdInstance = instance;
        configuratorMock.Setup(p => p.AttachAt(It.IsAny<DummyTransactionMiddleware>(), ActionPosition.Default, ActionPosition.Default))
            .Callback(OnAttach)
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.AttachAt<DummyTransactionMiddleware>(ActionPosition.Default, ActionPosition.Default);
        Assert.That(createdInstance, Is.InstanceOf<DummyTransactionMiddleware>().And.Not.Null);
        configuratorMock.Verify();
    }

    [Test]
    public void AttachAt_T_with_ActionPosition_creates_new_middleware_instance()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        DummyTransactionMiddleware? createdInstance = null;
        void OnAttach(DummyTransactionMiddleware instance, in ActionPosition a, in ActionPosition b) => createdInstance = instance;
        configuratorMock.Setup(p => p.AttachAt(It.IsAny<DummyTransactionMiddleware>(), ActionPosition.Default, ActionPosition.Default))
            .Callback(OnAttach)
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.AttachAt<DummyTransactionMiddleware>(ActionPosition.Default);
        Assert.That(createdInstance, Is.InstanceOf<DummyTransactionMiddleware>().And.Not.Null);
        configuratorMock.Verify();
    }


    [Test]
    public void AttachAt_T_with_out_T_ActionPosition_ActionPosition_creates_new_middleware_instance()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        DummyTransactionMiddleware? createdInstance = null;
        void OnAttach(DummyTransactionMiddleware instance, in ActionPosition a, in ActionPosition b) => createdInstance = instance;
        configuratorMock.Setup(p => p.AttachAt(It.IsAny<DummyTransactionMiddleware>(), ActionPosition.Default, ActionPosition.Default))
            .Callback(OnAttach)
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.AttachAt<DummyTransactionMiddleware>(out DummyTransactionMiddleware? outResult, ActionPosition.Default, ActionPosition.Default);
        Assert.That(createdInstance, Is.InstanceOf<DummyTransactionMiddleware>().And.Not.Null);
        Assert.That(createdInstance, Is.SameAs(outResult));
        configuratorMock.Verify();
    }

    [Test]
    public void AttachAt_T_with_out_T_ActionPosition_creates_new_middleware_instance()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        DummyTransactionMiddleware? createdInstance = null;
        void OnAttach(DummyTransactionMiddleware instance, in ActionPosition a, in ActionPosition b) => createdInstance = instance;
        configuratorMock.Setup(p => p.AttachAt(It.IsAny<DummyTransactionMiddleware>(), ActionPosition.Default, ActionPosition.Default))
            .Callback(OnAttach)
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.AttachAt<DummyTransactionMiddleware>(out DummyTransactionMiddleware? outResult, ActionPosition.Default);
        Assert.That(createdInstance, Is.InstanceOf<DummyTransactionMiddleware>().And.Not.Null);
        Assert.That(createdInstance, Is.SameAs(outResult));
        configuratorMock.Verify();
    }

    [Test]
    public void AttachAt_T_with_T_ActionPosition_registers_at_default_position()
    {
        var configuratorMock = new Mock<IMiddlewareConfigurator>() { CallBase = true };
        DummyTransactionMiddleware? middleware = new();
        configuratorMock.Setup(p => p.AttachAt(middleware, ActionPosition.Default, ActionPosition.Default))
            .Returns(configuratorMock.Object)
            .Verifiable(Times.Once);
        _ = configuratorMock.Object.AttachAt(middleware, ActionPosition.Default);
        configuratorMock.Verify();
    }
}
