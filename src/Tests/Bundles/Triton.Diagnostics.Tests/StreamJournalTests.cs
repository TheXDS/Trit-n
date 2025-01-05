#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Diagnostics;

public class StreamJournalTests : JournalTestsBase
{
    [TestCaseSource(nameof(GetTestCases))]
    public void Journal_writes_data(CrudAction action, bool withEntity, bool withSettings)
    {
        var actorMock = new Mock<IActorProvider>();
        actorMock.Setup(p => p.GetCurrentActor()).Returns("Test user").Verifiable(withSettings ? Times.Once : Times.Never);
        JournalSettings s = withSettings ? new JournalSettings { ActorProvider = actorMock.Object } : new JournalSettings();
        var buff = new byte[1024];
        using var ms = new MemoryStream(buff, true);
        StreamJournal j = new(() => ms);
        j.Log(action, withEntity ? [new ChangeTrackerItem(new User("test", "Test user"), new User("tst2", "Test user"))] : null, s);        
        Assert.That(System.Text.Encoding.UTF8.GetString(buff).Trim('\0').Length, Is.Not.Zero);
        actorMock.Verify();
    }
}