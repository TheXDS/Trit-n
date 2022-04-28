#pragma warning disable CS1591

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics
{
    public class ReadOnlySimulatorTests : MiddlewareTestsBase
    {
        protected static ServiceResult? RunSimulatorFail(IMiddlewareConfigurator testRepo, CrudAction action, Model? entity)
        {
            if (testRepo.GetRunner().RunProlog(action, entity) is { } pr) return pr;
            Assert.Fail();
            return testRepo.GetRunner().RunEpilog(action, entity);
        }

        protected static (ServiceResult?, bool) RunSimulatorPass(IMiddlewareConfigurator testRepo, CrudAction action, Model? entity)
        {
            if (testRepo.GetRunner().RunProlog(action, entity) is { } pr) return (pr, false);
            return (testRepo.GetRunner().RunEpilog(action, entity), true);
        }

        [Test]
        public void Simulator_blocks_action()
        {
            static ServiceResult? CheckBlocked(CrudAction crudAction, Model? entity)
            {
                Assert.Fail();
                return null;
            }
            var t = new TransactionConfiguration().UseSimulation(false).AddEpilog(CheckBlocked);
            RunSimulatorFail(t, CrudAction.Create, new User("x", "test"));
            RunSimulatorFail(t, CrudAction.Update, new User("x", "test"));
            RunSimulatorFail(t, CrudAction.Delete, new User("x", "test"));
            RunSimulatorFail(t, CrudAction.Commit, new User("x", "test"));
        }

        [Test]
        public void Simulator_allows_Read()
        {
            bool ranEpilog = false;
            ServiceResult? ChkEpilog(CrudAction crudAction, Model? entity)
            {
                ranEpilog = true;
                return null;
            }
            var t = new TransactionConfiguration().UseSimulation(false).AddEpilog(ChkEpilog);
            Assert.IsTrue(RunSimulatorPass(t, CrudAction.Read, new User("x", "test")).Item2);
            Assert.True(ranEpilog);
        }

        [TestCase(CrudAction.Create, false)]
        [TestCase(CrudAction.Update, false)]
        [TestCase(CrudAction.Delete, false)]
        [TestCase(CrudAction.Commit, false)]
        [TestCase(CrudAction.Read, true)]
        public void Simulator_runs_epilogs(CrudAction action, bool ranTrans)
        {
            bool ranEpilog = false;
            ServiceResult? ChkEpilog(CrudAction crudAction, Model? entity)
            {
                ranEpilog = true;
                return null;
            }
            var t = new TransactionConfiguration().UseSimulation().AddEpilog(ChkEpilog);
            Assert.AreEqual(ranTrans, RunSimulatorPass(t, action, new User("x", "test")).Item2);
            Assert.True(ranEpilog);
        }
    }

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
}
