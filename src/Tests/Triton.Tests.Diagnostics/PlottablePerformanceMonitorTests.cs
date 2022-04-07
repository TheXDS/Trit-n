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
            await Run(testRepo, CrudAction.Commit, 1000);
            await Run(testRepo, CrudAction.Commit, 2000);
            await Run(testRepo, CrudAction.Commit, 3000);
            var evts = perfMon.Events.ToArray();
            Assert.AreEqual(3, perfMon.EventCount);
            Assert.AreEqual(3, evts.Length);
            Assert.True(evts[0] >= 1000 && evts[0] <= 2000);
            Assert.True(evts[1] >= 2000 && evts[1] <= 3000);
            Assert.True(evts[2] >= 3000 && evts[2] <= 4000);
        }
    }
}