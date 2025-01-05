#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using TheXDS.Triton.Diagnostics.Extensions;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public class JournalMiddlewareTests
{
    [ExcludeFromCodeCoverage]
    private class TestJournal : IJournalMiddleware
    {
        public void Log(CrudAction action, IEnumerable<ChangeTrackerItem>? changeSet, JournalSettings settings)
        {
        }
    }

    [Test]
    public void UseJournal_registers_middleware_as_last_Epilogue()
    {
        var configMock = new Mock<IMiddlewareConfigurator>();
        var r = configMock.Object.UseJournal<TestJournal>();
        configMock.Verify(x => x.AddLateEpilogue(It.IsAny<MiddlewareAction>()), Times.Once());
    }

    [Test]
    public void UseJournal_with_settings_registers_new_journal()
    {
        var configMock = new Mock<IMiddlewareConfigurator>();
        var r = configMock.Object.UseJournal<TestJournal>(new JournalSettings());
        configMock.Verify(x => x.AddLateEpilogue(It.IsAny<MiddlewareAction>()), Times.Once());
    }

    [Test]
    public void UseJournal_with_singleton_registers_middleware_as_last_Epilogue()
    {
        var configMock = new Mock<IMiddlewareConfigurator>();        
        var r = configMock.Object.UseJournal(Mock.Of<IJournalMiddleware>(MockBehavior.Loose));
        configMock.Verify(x => x.AddLateEpilogue(It.IsAny<MiddlewareAction>()), Times.Once());
    }

    [Test]
    public void UseJournal_with_singleton_and_settings_registers_new_journal()
    {
        var configMock = new Mock<IMiddlewareConfigurator>();
        var r = configMock.Object.UseJournal(Mock.Of<IJournalMiddleware>(MockBehavior.Loose), new JournalSettings());
        configMock.Verify(x => x.AddLateEpilogue(It.IsAny<MiddlewareAction>()), Times.Once());
    }

    [Test]
    public void Journal_write_test()
    {
        var journalMock = new Mock<IJournalMiddleware>();
        journalMock.Setup(p => p.Log(It.IsAny<CrudAction>(), It.IsAny<IEnumerable<ChangeTrackerItem>?>(), It.IsAny<JournalSettings>())).Verifiable(Times.Once);
        var r = new TransactionConfiguration().UseJournal(journalMock.Object);
        _ = r.GetRunner().RunEpilogue(default, null);
        journalMock.Verify();
    }

    [Test]
    public void Journal_exception_still_succeeds_operation()
    {
        var journalMock = new Mock<IJournalMiddleware>();
        journalMock.Setup(p => p.Log(It.IsAny<CrudAction>(), It.IsAny<IEnumerable<ChangeTrackerItem>?>(), It.IsAny<JournalSettings>())).Throws<InvalidProgramException>();
        var r = new TransactionConfiguration().UseJournal(journalMock.Object);
        var result = r.GetRunner().RunEpilogue(default, null);
        Assert.That(result!.Success);
    }
}