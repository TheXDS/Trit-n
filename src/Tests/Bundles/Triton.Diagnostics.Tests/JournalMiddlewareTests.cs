#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Diagnostics;

public class JournalMiddlewareTests : MiddlewareTestsBase
{
    private class TestJournal : IJournalMiddleware
    {
        public record Entry(CrudAction Action, Model? Entity, JournalSettings Settings);

        public List<Entry> Entries { get; } = [];

        public void Log(CrudAction action, IEnumerable<Model>? entity, JournalSettings settings)
        {
            if (entity is not null) Entries.AddRange(entity.Select(p => new Entry(action, p, settings)));
        }
    }

    private class BrokenJournal : IJournalMiddleware
    {
        public void Log(CrudAction action, IEnumerable<Model>? entity, JournalSettings settings)
        {
            throw new Exception("Test");
        }
    }

    [Test]
    public void UseJournal_registers_middleware_as_last_epilog()
    {
        var configMock = new Mock<IMiddlewareConfigurator>();
        var r = configMock.Object.UseJournal<TestJournal>();
        configMock.Verify(x => x.AddLastEpilog(It.IsAny<MiddlewareAction>()), Times.Once());
    }

    [Test]
    public void UseJournal_with_settings_registers_new_journal()
    {
        var configMock = new Mock<IMiddlewareConfigurator>();
        var r = configMock.Object.UseJournal<TestJournal>(new JournalSettings());
        configMock.Verify(x => x.AddLastEpilog(It.IsAny<MiddlewareAction>()), Times.Once());
    }

    [Test]
    public void UseJournal_with_singleton_registers_journal()
    {
        var configMock = new Mock<IMiddlewareConfigurator>();
        var journal = new TestJournal();
        var r = configMock.Object.UseJournal(journal);
        configMock.Verify(x => x.AddLastEpilog(It.IsAny<MiddlewareAction>()), Times.Once());
    }

    [Test]
    public async Task Journal_write_test()
    {
        var j = new TestJournal();
        var r = new TransactionConfiguration().UseJournal(j);
        var u = new User("1", "Test");
        await Run(r, CrudAction.Read, [u]);
        Assert.That(1, Is.EqualTo(j.Entries.Count));
        Assert.That(u, Is.SameAs(j.Entries[0].Entity));
        Assert.That(CrudAction.Read, Is.EqualTo(j.Entries[0].Action));
    }

    [Test]
    public async Task Journal_exception_test()
    {
        var r = new TransactionConfiguration().UseJournal<BrokenJournal>();
        var u = new User("1", "Test");
        var result = await Run(r, CrudAction.Read, [u]);
        Assert.That(result,Is.Not.Null);
        Assert.That(result!.Success);
    }
}