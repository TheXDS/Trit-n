#pragma warning disable CS1591

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics
{
    public class PlottablePerformanceMonitorTests : PerformanceMonitorTestsBase<PlottablePerfMonitor>
    {
        protected override IEnumerable<string> ExtraTelemetryNpcProps()
        {
            yield return nameof(PlottablePerfMonitor.Events);
        }

        [Test]
        public async Task Monitor_exposes_events()
        {
            var (testRepo, perfMon) = Build();
            await Run(testRepo, CrudAction.Commit, 1000);
            await Run(testRepo, CrudAction.Commit, 2000);
            await Run(testRepo, CrudAction.Commit, 3000);
            var evts = perfMon.Events.ToArray();
            Assert.AreEqual(3, perfMon.EventCount);
            Assert.AreEqual(3, evts.Length);
            System.Console.WriteLine(evts[0]);
            Assert.True(evts[0] >= 1000 && evts[0] <= 2000);
            System.Console.WriteLine(evts[1]);
            Assert.True(evts[1] >= 2000 && evts[1] <= 3000);
            System.Console.WriteLine(evts[2]);
            Assert.True(evts[2] >= 3000 && evts[2] <= 4000);
        }
    }
}