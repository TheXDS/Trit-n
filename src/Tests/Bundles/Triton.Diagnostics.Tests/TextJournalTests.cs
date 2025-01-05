using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public class TextJournalTests
{
    private class TestTextJournal : TextJournal
    {
        public int WriteTextInvokeCount { get; private set; }

        public Action<IEnumerable<string>>? OnWriteText { get; set; }

        protected override void WriteText(IEnumerable<string> lines)
        {
            WriteTextInvokeCount++;
            OnWriteText?.Invoke(lines);
        }
    }

    [Test]
    public void Log_writes_messages_without_IActorProvider()
    {
        IEnumerable<string> messages = [];
        var journal = new TestTextJournal() { OnWriteText = l => messages = l };
        journal.Log(CrudAction.Commit, null, default);
        Assert.That(messages, Is.Not.Empty);
        Assert.That(journal.WriteTextInvokeCount, Is.EqualTo(1));
    }
}
