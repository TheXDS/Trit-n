#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Diagnostics;

public class TextFileJournalTests
{
    private class TestActorProvider : IActorProvider
    {
        public string? GetCurrentActor() => "Test executor";
    }

    private class TestOldValueProvider : IOldValueProvider
    {
        public IEnumerable<KeyValuePair<PropertyInfo, object?>>? GetOldValues(Model? entity)
        {
            if (entity is null) yield break;
            foreach (var j in entity.GetType().GetProperties().Where(p => p.CanRead))
            {
                yield return new KeyValuePair<PropertyInfo, object?>(j, j.PropertyType.Default());
            }
        }
    }

    [TestCase(CrudAction.Create, true, false)]
    [TestCase(CrudAction.Read, true, false)]
    [TestCase(CrudAction.Update, true, false)]
    [TestCase(CrudAction.Delete, true, false)]
    [TestCase(CrudAction.Commit, true, false)]
    [TestCase(CrudAction.Create, false, false)]
    [TestCase(CrudAction.Read, false, false)]
    [TestCase(CrudAction.Update, false, false)]
    [TestCase(CrudAction.Delete, false, false)]
    [TestCase(CrudAction.Commit, false, false)]
    [TestCase(CrudAction.Create, true, true)]
    [TestCase(CrudAction.Read, true, true)]
    [TestCase(CrudAction.Update, true, true)]
    [TestCase(CrudAction.Delete, true, true)]
    [TestCase(CrudAction.Commit, true, true)]
    [TestCase(CrudAction.Create, false, true)]
    [TestCase(CrudAction.Read, false, true)]
    [TestCase(CrudAction.Update, false, true)]
    [TestCase(CrudAction.Delete, false, true)]
    [TestCase(CrudAction.Commit, false, true)]
    public void Journal_writes_data(CrudAction action, bool withEntity, bool withSettings)
    {
        string p = Path.GetTempFileName();
        JournalSettings s = withSettings
            ? new JournalSettings
            {
                ActorProvider = new TestActorProvider(),
                OldValueProvider = new TestOldValueProvider()
            }
            : new JournalSettings();
        
        TextFileJournal j = new() { Path = p };
        j.Log(action, withEntity ? new User("test", "Test user") : null, s);
        FileInfo f = new(p);
        Assert.NotZero(f.Length);
        f.Delete();
    }

    [Test]
    public void Journal_disables_itself_on_exception()
    {
        string invalidPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(invalidPath);
        TextFileJournal j = new() { Path = invalidPath };
        Assert.AreEqual(invalidPath, j.Path);
        Assert.Throws<UnauthorizedAccessException>(()=>j.Log(CrudAction.Commit, null, new()));
        Assert.IsNull(j.Path);
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