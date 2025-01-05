#pragma warning disable CS1591

using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.ComponentModel;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public abstract class PerformanceMonitorTestsBase<T> : MiddlewareTestsBase<T> where T : PerformanceMonitorBase, new()
{
    protected const int EventsEpsilon = 200;

    protected virtual IEnumerable<string> ExtraTelemetryNpcProps() => [];

    protected void RunCrudAction(IMiddlewareRunner runner, int sleep)
    {
        runner.RunPrologue(CrudAction.Commit, null);
        Thread.Sleep(sleep);
        runner.RunEpilogue(CrudAction.Commit, null);
    }

    protected RangeConstraint IsAround(int meanMs, int epsilon = EventsEpsilon)
    {
        return Is.InRange(meanMs - epsilon, meanMs + epsilon);
    }

    [Test]
    public void Monitor_registers_event()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, 1000);
        Assert.That(perfMon.EventCount, Is.EqualTo(1));
        Assert.That(perfMon.AverageMs, IsAround(1000));
    }

    [Test]
    public void Monitor_includes_commits()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, CrudAction.Commit);
        Assert.That(perfMon.EventCount, Is.EqualTo(1));
    }

    [Test]
    public void Monitor_includes_reads()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, CrudAction.Read);
        Assert.That(perfMon.EventCount, Is.EqualTo(1));
    }

    [Test]
    public void Monitor_skips_create()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, CrudAction.Create);
        Assert.That(perfMon.EventCount, Is.Zero);
    }

    [Test]
    public void Monitor_skips_update()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, CrudAction.Update);
        Assert.That(perfMon.EventCount, Is.Zero);
    }

    [Test]
    public void Monitor_skips_delete()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, CrudAction.Delete);
        Assert.That(perfMon.EventCount, Is.Zero);
    }

    [Test]
    public void Monitor_data_is_initially_NaN()
    {
        var perfMon = new T();
        Assert.That(perfMon.AverageMs, Is.NaN);
        Assert.That(perfMon.MinMs, Is.NaN);
        Assert.That(perfMon.MaxMs, Is.NaN);
    }

    [Test]
    public void Monitor_initially_does_not_include_any_event()
    {
        var perfMon = new T();
        Assert.That(perfMon.EventCount, Is.Zero);
    }

    [Test]
    public void Monitor_has_full_telemetry()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, 2000);
        RunCrudAction(runner, 1000);
        Assert.That(perfMon.AverageMs, IsAround(1500));
        Assert.That(perfMon.MinMs, IsAround(1000));
        Assert.That(perfMon.MaxMs, IsAround(2000));
    }

    [Test]
    public void Monitor_supports_reset()
    {
        (var runner, var perfMon) = Build();
        RunCrudAction(runner, 1000);
        perfMon.Reset();
        Assert.That(perfMon.EventCount, Is.Zero);
        Assert.That(perfMon.AverageMs, Is.NaN);
        Assert.That(perfMon.MinMs, Is.NaN);
        Assert.That(perfMon.MaxMs, Is.NaN);
    }

    [Test]
    public void Monitor_fires_npc_events_for_telemetry()
    {
        (var runner, var perfMon) = Build();
        List<string> pendingProps = new(
        [
            nameof(perfMon.EventCount),
            nameof(perfMon.AverageMs),
            nameof(perfMon.MinMs),
            nameof(perfMon.MaxMs),
            ..ExtraTelemetryNpcProps()
        ]);
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Assert.That(sender, Is.SameAs(perfMon));
            Assert.That(pendingProps.Remove(e.PropertyName!));
        }
        perfMon.PropertyChanged += OnPropertyChanged;
        RunCrudAction(runner);
        perfMon.PropertyChanged -= OnPropertyChanged;
        Assert.That(pendingProps, Is.Empty);
    }
}