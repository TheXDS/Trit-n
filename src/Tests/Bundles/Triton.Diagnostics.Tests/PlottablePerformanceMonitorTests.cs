#pragma warning disable CS1591

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TheXDS.MCART.Helpers;
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
            Assert.True(evts[0] >= 900 && evts[0] <= 2100);
            System.Console.WriteLine(evts[1]);
            Assert.True(evts[1] >= 1900 && evts[1] <= 3100);
            System.Console.WriteLine(evts[2]);
            Assert.True(evts[2] >= 2900 && evts[2] <= 4100);
        }

        [Test]
        public async Task Monitor_removes_old_data()
        {
            PlottablePerfMonitor p = new () { MaxSamples = 5 };
            ITransactionMiddleware perfMon = p;
            
            Assert.AreEqual(5, p.MaxSamples);
            for (var j = 0; j < 6; j++)
            {
                perfMon.PrologAction(CrudAction.Commit, null);
                await Task.Delay(500 * j);
                perfMon.EpilogAction(CrudAction.Commit, null);
            }
            Assert.AreEqual(5, p.EventCount);
            Assert.AreEqual(5, p.Events.Count());
            Assert.IsTrue(p.Events.Last().IsBetween(2250, 2750));
            Assert.IsTrue(p.Events.First().IsBetween(250, 750));

            p.MaxSamples = 2;
            Assert.AreEqual(2, p.EventCount);
            Assert.AreEqual(2, p.Events.Count());
            Assert.IsTrue(p.Events.Last().IsBetween(2250, 2750));
            Assert.IsTrue(p.Events.First().IsBetween(1750, 2250));
        }
    }
}