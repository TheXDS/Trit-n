#pragma warning disable CS1591

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.ServicePool.Triton.Tests;

public class Tests
{
    [Test]
    public void Basic_ServicePool_registration_Test()
    {
        ServicePool testPool = new();
        Assert.That(testPool.UseTriton(),Is.InstanceOf<ITritonConfigurable>());
        Assert.That(testPool.Resolve<ITritonConfigurable>(), Is.Not.Null);
    }

    [Test]
    public void Configurable_references_pool()
    {
        ServicePool testPool = new();
        var c = testPool.UseTriton();
        Assert.That(c, Is.InstanceOf<ITritonConfigurable>());
        Assert.That(c.Pool, Is.SameAs(testPool));
    }

    [Test]
    public void DiscoverContexts_finds_context()
    {
        ServicePool testPool = new();
        testPool.UseTriton().DiscoverContexts();
        var s = testPool.ResolveAll<TritonService>().Select(p => p.Factory).ToArray();
        Assert.That(s.Any(p => p is EfCoreTransFactory<TestDbContext>), Is.True);
    }

    [Test]
    public void UseContext_with_generic_overload_registers_context()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext<TestDbContext>();
        var s = testPool.ResolveAll<TritonService>().Select(p => p.Factory).ToArray();
        Assert.That(s.Any(p => p is EfCoreTransFactory<TestDbContext>), Is.True);
    }

    [Test]
    public void UseContext_contract_test()
    {
        ServicePool testPool = new();
        Assert.Throws<ArgumentException>(() => testPool.UseTriton().UseContext(typeof(int)));
    }

    [Test]
    public void UseContext_registers_context_explicitly()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext(typeof(TestDbContext));
        var s = testPool.ResolveAll<TritonService>().Select(p => p.Factory).ToArray();
        Assert.That(s.Any(p => p is EfCoreTransFactory<TestDbContext>), Is.True);
    }

    [Test]
    public void UseContext_with_static_options_creates_transactions()
    {
        ServicePool testPool = new();
        DbContextOptions options = new DbContextOptionsBuilder().Options;
        testPool.UseTriton().UseContext(typeof(ConfigurableContext), options);
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.That(s.Any(p => p.GetReadTransaction() is not null), Is.True);
    }

    [Test]
    public void UseContext_with_config_method_creates_transactions()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext(typeof(ConfigurableContext), (DbContextOptionsBuilder _) => { });
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.That(s.Any(p => p.GetReadTransaction() is not null), Is.True);
    }

    [Test]
    public void UseContext_generic_with_static_options_creates_transactions()
    {
        ServicePool testPool = new();
        DbContextOptions<ConfigurableContext> options = new DbContextOptionsBuilder<ConfigurableContext>().Options;
        testPool.UseTriton().UseContext<ConfigurableContext>(options);
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.That(s.Any(p => p.GetReadTransaction() is not null), Is.True);
    }

    [Test]
    public void UseContext_generic_with_config_method_creates_transactions()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext((DbContextOptionsBuilder<ConfigurableContext> _) => { });
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.That(s.Any(p => p.GetReadTransaction() is not null), Is.True);
    }

    [Test]
    public void UseMiddleware_with_out_parameter_registers_middleware()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseMiddleware<TestMiddleware>(out var m);
        Assert.That(m.PrologRan, Is.False);
        testPool.Resolve<IMiddlewareRunner>()?.RunProlog(CrudAction.Commit, null);
        Assert.That(m.PrologRan, Is.True);
        testPool.Resolve<IMiddlewareRunner>()?.RunEpilog(CrudAction.Commit, null);
        Assert.That(m.EpilogRan, Is.True);
    }
    [Test]
    public void UseTransactionPrologs_registers_actions()
    {
        bool actionRan = false;
        ServiceResult? TestAction(CrudAction action, IEnumerable<Model>? entity)
        {
            actionRan = true;
            return null;
        }
        
        ServicePool testPool = new();
        testPool.UseTriton().UseTransactionPrologs(TestAction);
        Assert.That(actionRan, Is.False);
        testPool.Resolve<IMiddlewareRunner>()?.RunProlog(CrudAction.Commit, null);
        Assert.That(actionRan, Is.True);
    }
    
    [Test]
    public void UseTransactionEpilogs_registers_actions()
    {
        bool actionRan = false;
        ServiceResult? TestAction(CrudAction action, IEnumerable<Model>? entity)
        {
            actionRan = true;
            return null;
        }
        
        ServicePool testPool = new();
        testPool.UseTriton().UseTransactionEpilogs(TestAction);
        Assert.That(actionRan, Is.False);
        testPool.Resolve<IMiddlewareRunner>()?.RunEpilog(CrudAction.Commit, null);
        Assert.That(actionRan, Is.True);
    }

    [Test]
    public void UseTriton_with_config_callback()
    {
        var configRan = false;
        ServicePool testPool = new();
        Assert.That(testPool.UseTriton(p =>
        {
            Assert.That(p.GetType().Implements<ITritonConfigurable>());
            configRan = true;
        }), Is.SameAs(testPool));
        Assert.That(configRan, Is.True);
    }

    [Test]
    public void Multiple_UseTriton_calls_doesnt_duplicate_singletons()
    {
        ServicePool testPool = new();
        testPool.UseTriton();
        var tc = testPool.Resolve<ITritonConfigurable>();
        Assert.That(tc, Is.Not.Null);
        testPool.UseTriton(p => Assert.That(tc, Is.SameAs(p)));
        Assert.That(testPool.ResolveAll<ITritonConfigurable>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void ResolveTritonService_resolves_service_for_context()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext<TestDbContext>();
        Assert.That(testPool.ResolveTritonService<TestDbContext>(), Is.AssignableFrom<TritonService>());
    }

    [Test]
    public void ResolveTritonService_initializes_all_required_singletons_if_none_registered()
    {
        ServicePool testPool = new();
        Assert.That(testPool.ResolveTritonService<TestDbContext>(), Is.AssignableFrom<TritonService>());
    }

    [Test]
    public void ConfigureMiddlewares_supports_callback()
    {
        new ServicePool().UseTriton().ConfigureMiddlewares(p => Assert.That(p, Is.Not.Null));
    }
    
    [Test]
    public void ConfigureMiddlewares_contract_test()
    {
        var tc = new ServicePool().UseTriton();
        Assert.Throws<ArgumentNullException>(() => tc.ConfigureMiddlewares(null!));
    }

    [ExcludeFromCodeCoverage]
    public class TestMiddleware : ITransactionMiddleware
    {
        public bool PrologRan { get; set; }
        public bool EpilogRan { get; set; }

        ServiceResult? ITransactionMiddleware.PrologAction(CrudAction action, IEnumerable<Model>? entity)
        {
            PrologRan = true;
            return null;
        }

        ServiceResult? ITransactionMiddleware.EpilogAction(CrudAction action, IEnumerable<Model>? entity)
        {
            EpilogRan = true;
            return null;
        }
    }
    
    [ExcludeFromCodeCoverage]
    public class TestDbContext : DbContext
    {
        public DbSet<TestModel> Tests { get; set; } = null!;
    }

    [ExcludeFromCodeCoverage]
    public class ConfigurableContext : DbContext
    {
        public ConfigurableContext(DbContextOptions options) : base(options)
        {
            Options = options;
        }

        public DbSet<TestModel> Tests { get; set; } = null!;
        public DbContextOptions Options { get; }
    }

    [ExcludeFromCodeCoverage]
    public class TestModel : Model<int>
    {
        public string Name { get; set; } = null!;
    }
}