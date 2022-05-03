#pragma warning disable CS1591

namespace TheXDS.Triton.Tests.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using Models;

public class JournalMiddlewareTests : MiddlewareTestsBase
{
    private class TestJournal : IJournalMiddleware
    {
        public record Entry(CrudAction Action, Model? Entity, JournalSettings Settings);

        public List<Entry> Entries { get; } = new();
        public void Log(CrudAction action, Model? entity, JournalSettings settings)
        {
            Entries.Add(new(action, entity, settings));
        }
    }

    private class BrokenJournal : IJournalMiddleware
    {
        public void Log(CrudAction action, Model? entity, JournalSettings settings)
        {
            throw new Exception("Test");
        }
    }

    [Test]
    public async Task Journal_write_test()
    {
        var j = new TestJournal();
        var r = new TransactionConfiguration().UseJournal(j);
        var u = new User("1", "Test");
        await Run(r, CrudAction.Read, u);
        Assert.AreEqual(1, j.Entries.Count);
        Assert.AreSame(u, j.Entries[0].Entity);
        Assert.AreEqual(CrudAction.Read, j.Entries[0].Action);
    }

    [Test]
    public async Task Journal_exception_test()
    {
        var r = new TransactionConfiguration().UseJournal<BrokenJournal>();
        var u = new User("1", "Test");
        var result = await Run(r, CrudAction.Read, u);
        Assert.NotNull(result);
        Assert.IsTrue(result!.Success);
    }
}