#pragma warning disable CS1591

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Diagnostics;

namespace Triton.Tests.Diagnostics
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
            (var testRepo, var perfMon) = Build();
            await Run(testRepo, CrudAction.Commit, 100);
            await Run(testRepo, CrudAction.Commit, 200);
            await Run(testRepo, CrudAction.Commit, 300);
            var evts = perfMon.Events.ToArray();
            Assert.AreEqual(3, perfMon.EventCount);
            Assert.AreEqual(3, evts.Length);
            Assert.True(evts[0] >= 100 && evts[0] <= 200);
            Assert.True(evts[1] >= 200 && evts[1] <= 300);
            Assert.True(evts[2] >= 300 && evts[2] <= 400);
        }
    }
}