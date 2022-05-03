#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Services;

namespace TheXDS.ServicePool.Triton.Tests;

public class Tests
{
    [Test]
    public void Basic_ServicePool_registration_Test()
    {
        ServicePool testPool = new();
        Assert.IsInstanceOf<ITritonConfigurable>(testPool.UseTriton());
        Assert.IsNotNull(testPool.Resolve<IMiddlewareConfigurator>());
    }
    
    [Test]
    public void ServicePool_registration_with_config_callback_Test()
    {
        ServicePool testPool = new();
        Assert.IsInstanceOf<ITritonConfigurable>(testPool.UseTriton(Assert.IsInstanceOf<IMiddlewareConfigurator>));
        Assert.IsNotNull(testPool.Resolve<IMiddlewareConfigurator>());
    }
    
    [Test]
    public void ServicePool_registration_picks_up_config_in_pool_Test()
    {
        ServicePool testPool = new();
        IMiddlewareConfigurator conf = new TransactionConfiguration();
        testPool.RegisterNow(conf);
        
        Assert.IsInstanceOf<ITritonConfigurable>(testPool.UseTriton(c => Assert.AreSame(c, conf)));
        Assert.AreSame(conf, testPool.Resolve<IMiddlewareConfigurator>());
    }
}