#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Helpers;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

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
        Assert.That(perfMon.EventCount, Is.EqualTo(3));
        Assert.That(evts.Length, Is.EqualTo(3));
        Assert.That(evts[0] >= 900 && evts[0] <= 2100);
        Assert.That(evts[1] >= 1900 && evts[1] <= 3100);
        Assert.That(evts[2] >= 2900 && evts[2] <= 4100);
    }

    [Test]
    public async Task Monitor_removes_old_data()
    {
        PlottablePerfMonitor p = new () { MaxSamples = 5 };
        ITransactionMiddleware perfMon = p;
        
        Assert.That(p.MaxSamples, Is.EqualTo(5));
        for (var j = 0; j < 6; j++)
        {
            perfMon.PrologAction(CrudAction.Commit, null);
            await Task.Delay(500 * j);
            perfMon.EpilogAction(CrudAction.Commit, null);
        }
        Assert.That(p.EventCount, Is.EqualTo(5));
        Assert.That(p.Events.Count(), Is.EqualTo(5));
        Assert.That(p.Events.Last().IsBetween(2250, 2750));
        Assert.That(p.Events.First().IsBetween(250, 750));

        p.MaxSamples = 2;
        Assert.That(p.EventCount, Is.EqualTo(2));
        Assert.That(p.Events.Count(), Is.EqualTo(2));
        Assert.That(p.Events.Last().IsBetween(2250, 2750));
        Assert.That(p.Events.First().IsBetween(1750, 2250));
    }
}