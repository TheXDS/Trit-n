#pragma warning disable CS1591

using NUnit.Framework;
using System.Diagnostics;
using TheXDS.Triton.Diagnostics.Middleware;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Diagnostics;

public class DelaySimulatorTests
{
    [Test]
    public void Instance_test()
    {
        var j = new DelaySimulator();
        Assert.That(j, Is.InstanceOf<ITransactionMiddleware>());
        Assert.That(j.DelayRange.Minimum, Is.EqualTo(500));
        Assert.That(j.DelayRange.Maximum, Is.EqualTo(1500));
    }

    [Test]
    public void Instance_with_parameters_test()
    {
        var j = new DelaySimulator(100, 200);
        Assert.That(j, Is.InstanceOf<ITransactionMiddleware>());
        Assert.That(j.DelayRange.Minimum, Is.EqualTo(100));
        Assert.That(j.DelayRange.Maximum, Is.EqualTo(200));
    }

    [Test]
    public void Middleware_delays_execution_on_Prologue()
    {
        IMiddlewareConfigurator r = new TransactionConfiguration();

        var s = Stopwatch.StartNew();
        r.GetRunner().RunPrologue(CrudAction.Read, null);
        r.GetRunner().RunEpilogue(CrudAction.Read, null);
        s.Stop();
        var time1 = s.ElapsedMilliseconds;

        r.Attach<DelaySimulator>();
        s.Restart();
        r.GetRunner().RunPrologue(CrudAction.Read, null);
        r.GetRunner().RunEpilogue(CrudAction.Read, null);
        s.Stop();
        var time2 = s.ElapsedMilliseconds;

        Assert.That(time1, Is.LessThan(time2));
    }
}
