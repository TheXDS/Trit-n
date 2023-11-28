#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

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
        Assert.That(0, Is.EqualTo(perfMon.EventCount));
        await Run(testRepo, CrudAction.Commit, 1000);
        Assert.That(1, Is.EqualTo(perfMon.EventCount));
        Assert.That(perfMon.AverageMs > 500 && perfMon.AverageMs < 1500);
    }

    [Test]
    public async Task Monitor_includes_commits_and_reads()
    {
        (var testRepo, var perfMon) = Build();
        Assert.That(0, Is.EqualTo(perfMon.EventCount));
        await Run(testRepo, CrudAction.Read);
        await Run(testRepo, CrudAction.Commit);
        Assert.That(2, Is.EqualTo(perfMon.EventCount));
    }

    [Test]
    public async Task Monitor_skips_non_commits()
    {
        (var testRepo, var perfMon) = Build();
        Assert.That(0, Is.EqualTo(perfMon.EventCount));
        await Run(testRepo, CrudAction.Create);
        await Run(testRepo, CrudAction.Update);
        await Run(testRepo, CrudAction.Delete);
        Assert.That(0, Is.EqualTo(perfMon.EventCount));
    }

    [Test]
    public async Task Monitor_has_full_telemetry()
    {
        (var testRepo, var perfMon) = Build();
        Assert.That(0, Is.EqualTo(perfMon.EventCount));
        Assert.That(perfMon.AverageMs, Is.NaN);
        Assert.That(perfMon.MinMs, Is.NaN);
        Assert.That(perfMon.MaxMs, Is.NaN);
        await Run(testRepo, CrudAction.Commit, 2000);
        await Run(testRepo, CrudAction.Commit, 1500);
        Assert.That(2, Is.EqualTo(perfMon.EventCount));
        Assert.That(perfMon.AverageMs > 1600 && perfMon.AverageMs < 1900);
        Assert.That(perfMon.MinMs >= 1400 && perfMon.MinMs < 1900);
        Assert.That(perfMon.MaxMs, Is.GreaterThanOrEqualTo(1900));
        perfMon.Reset();
        Assert.That(0, Is.EqualTo(perfMon.EventCount));
        Assert.That(perfMon.AverageMs, Is.NaN);
        Assert.That(perfMon.MinMs, Is.NaN);
        Assert.That(perfMon.MaxMs, Is.NaN);
    }

    [Test]
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
            Assert.That(s, Is.SameAs(perfMon));
            Assert.That(expected.Remove(e.PropertyName!));
        };
        await Run(testRepo, CrudAction.Commit);
        Assert.That(expected, Is.Empty);
    }
}