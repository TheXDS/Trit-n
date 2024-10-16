#pragma warning disable CS1591

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
        JournalSettings s = withSettings
            ? new JournalSettings
            {
                ActorProvider = new TestActorProvider(),
                OldValueProvider = new TestOldValueProvider()
            }
            : new JournalSettings();

        string p = Path.GetTempFileName();
        StreamJournal j = new(() => File.Create(p));
        j.Log(action, withEntity ? new[] { new User("test", "Test user") } : null, s);
        FileInfo f = new(p);
        Assert.That(f.Length, Is.Not.Zero);
        f.Delete();
    }
}