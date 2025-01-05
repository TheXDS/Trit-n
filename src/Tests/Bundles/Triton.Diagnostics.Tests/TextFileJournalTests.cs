#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Diagnostics;

public class TextFileJournalTests : JournalTestsBase
{
    [TestCaseSource(nameof(GetTestCases))]
    public void Journal_writes_data(CrudAction action, bool withEntity, bool withSettings)
    {
        string p = Path.GetTempFileName();

        var actorMock = new Mock<IActorProvider>();
        actorMock.Setup(p => p.GetCurrentActor()).Returns("Test user").Verifiable(withSettings ? Times.Once : Times.Never);
        JournalSettings s = withSettings ? new JournalSettings { ActorProvider = actorMock.Object } : new JournalSettings();

        TextFileJournal j = new() { Path = p };
        j.Log(action, withEntity ? [new ChangeTrackerItem(new User("test", "Test user"), new User("test", "Test user"))] : null, s);
        FileInfo f = new(p);
        Assert.That(f.Length, Is.Not.Zero);
        f.Delete();
    }

    [Test]
    public void Journal_disables_itself_on_exception()
    {
        string invalidPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(invalidPath);
        TextFileJournal j = new() { Path = invalidPath };
        Assert.That(invalidPath, Is.EqualTo(j.Path));
        Assert.Throws<UnauthorizedAccessException>(() => j.Log(CrudAction.Commit, null, new()));
        Assert.That(j.Path, Is.Null);
        j.Log(CrudAction.Commit, null, new());
        Directory.Delete(invalidPath);
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("\0::?*")]
    public void Journal_Path_throws_on_invalid_path(string invalidPath)
    {
        TextFileJournal j = new();
        Assert.Throws<ArgumentException>(() => j.Path = invalidPath);
    }
}
