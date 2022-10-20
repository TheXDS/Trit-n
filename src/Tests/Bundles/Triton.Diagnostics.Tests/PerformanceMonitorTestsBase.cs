#pragma warning disable CS1591

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics
{
    public abstract class PerformanceMonitorTestsBase<T> : MiddlewareTestsBase<T> where T : PerformanceMonitorBase, new()
    {
        protected virtual IEnumerable<string> ExtraTelemetryNpcProps()
        {
            yield break;
        }

        [Test]
        public async Task Monitor_registers_event()
        {
            (var testRepo, var perfMon) = Build();
            Assert.AreEqual(0, perfMon.EventCount);
            await Run(testRepo, CrudAction.Commit, 1000);
            Assert.AreEqual(1, perfMon.EventCount);
            System.Console.WriteLine(perfMon.AverageMs);
            Assert.IsTrue(perfMon.AverageMs > 500 && perfMon.AverageMs < 1500);
        }

        [Test]
        public async Task Monitor_skips_non_commits()
        {
            (var testRepo, var perfMon) = Build();
            Assert.AreEqual(0, perfMon.EventCount);
            await Run(testRepo, CrudAction.Create);
            await Run(testRepo, CrudAction.Read);
            await Run(testRepo, CrudAction.Update);
            await Run(testRepo, CrudAction.Delete);
            await Run(testRepo, CrudAction.Commit);
            Assert.AreEqual(1, perfMon.EventCount);
        }

        [Test]
        public async Task Monitor_has_full_telemetry()
        {
            (var testRepo, var perfMon) = Build();
            Assert.AreEqual(0, perfMon.EventCount);
            Assert.IsNaN(perfMon.AverageMs);
            Assert.IsNaN(perfMon.MinMs);
            Assert.IsNaN(perfMon.MaxMs);
            await Run(testRepo, CrudAction.Commit, 2000);
            await Run(testRepo, CrudAction.Commit, 1500);
            Assert.AreEqual(2, perfMon.EventCount);
            System.Console.WriteLine(perfMon.AverageMs);
            Assert.IsTrue(perfMon.AverageMs > 1600 && perfMon.AverageMs < 1900);
            System.Console.WriteLine(perfMon.MinMs);
            Assert.IsTrue(perfMon.MinMs >= 1400 && perfMon.MinMs < 1900);
            System.Console.WriteLine(perfMon.MaxMs);
            Assert.GreaterOrEqual(perfMon.MaxMs, 1900);
            perfMon.Reset();
            Assert.AreEqual(0, perfMon.EventCount);
            Assert.IsNaN(perfMon.AverageMs);
            Assert.IsNaN(perfMon.MinMs);
            Assert.IsNaN(perfMon.MaxMs);
        }

        public async Task Monitor_fires_npc_events_for_telemetry()
        {
            (var testRepo, var perfMon) = Build();
            List<string> expected = new(new[]
            {
                nameof(perfMon.EventCount),
                nameof(perfMon.AverageMs),
                nameof(perfMon.MinMs),
                nameof(perfMon.MaxMs),
            }.Concat(ExtraTelemetryNpcProps()));
            perfMon.PropertyChanged += (s, e) =>
            {
                Assert.AreSame(s, perfMon);
                Assert.True(expected.Remove(e.PropertyName!));
            };
            await Run(testRepo, CrudAction.Commit);
            Assert.False(expected.Any());
        }
    }
}