#pragma warning disable CS1591

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Triton.Middleware;
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
            await Run(testRepo, CrudAction.Commit, 500);
            Assert.AreEqual(1, perfMon.EventCount);
            Assert.GreaterOrEqual(perfMon.AverageMs, 500.0);
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
            await Run(testRepo, CrudAction.Commit, 500);
            await Run(testRepo, CrudAction.Commit, 250);
            Assert.AreEqual(2, perfMon.EventCount);
            Assert.IsTrue(perfMon.AverageMs > 300 && perfMon.AverageMs < 400);
            Assert.IsTrue(perfMon.MinMs >= 250 && perfMon.MinMs < 500);
            Assert.GreaterOrEqual(perfMon.MaxMs, 500);
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