#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public class PlottablePerformanceMonitorTests : PerformanceMonitorTestsBase<PlottablePerfMonitor>
{
    protected override IEnumerable<string> ExtraTelemetryNpcProps() => [nameof(PlottablePerfMonitor.Events)];

    [Test]
    public void Monitor_events_match_event_count()
    {
        var (runner, perfMon) = Build();
        RunCrudAction(runner);
        RunCrudAction(runner);
        RunCrudAction(runner);
        Assert.That(perfMon.EventCount, Is.EqualTo(perfMon.Events.ToArray().Length));
    }

    [Test]
    public void Monitor_exposes_events()
    {
        var (runner, perfMon) = Build();
        RunCrudAction(runner, 1000);
        RunCrudAction(runner, 2000);
        RunCrudAction(runner, 3000);
        var evts = perfMon.Events.ToArray();
        Assert.That(evts[0], IsAround(1000));
        Assert.That(evts[1], IsAround(2000));
        Assert.That(evts[2], IsAround(3000));
    }

    [Test]
    public async Task Monitor_removes_old_data()
    {
        PlottablePerfMonitor p = new () { MaxSamples = 5 };
        ITransactionMiddleware perfMon = p;
        
        Assert.That(p.MaxSamples, Is.EqualTo(5));
        for (var j = 0; j <= 5; j++)
        {
            perfMon.PrologueAction(CrudAction.Commit, null);
            await Task.Delay(500 * j);
            perfMon.EpilogueAction(CrudAction.Commit, null);
        }
        Assert.That(p.EventCount, Is.EqualTo(5));
        Assert.That(p.Events.Last(), IsAround(2500));
        Assert.That(p.Events.First(), IsAround(500));

        p.MaxSamples = 2;
        Assert.That(p.EventCount, Is.EqualTo(2));
        Assert.That(p.Events.Last(), IsAround(2500));
        Assert.That(p.Events.First(), IsAround(2000));
    }
}