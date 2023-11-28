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
        Assert.IsInstanceOf<ITritonConfigurable>(testPool.UseTriton());
        Assert.IsNotNull(testPool.Resolve<ITritonConfigurable>());
    }

    [Test]
    public void Configurable_references_pool()
    {
        ServicePool testPool = new();
        var c = testPool.UseTriton();
        Assert.IsInstanceOf<ITritonConfigurable>(c);
        Assert.AreSame(testPool, c.Pool);
    }

    [Test]
    public void DiscoverContexts_finds_context()
    {
        ServicePool testPool = new();
        testPool.UseTriton().DiscoverContexts();
        var s = testPool.ResolveAll<TritonService>().Select(p => p.Factory).ToArray();
        Assert.IsTrue(s.Any(p => p is EfCoreTransFactory<TestDbContext>));
    }

    [Test]
    public void UseContext_with_generic_overload_registers_context()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext<TestDbContext>();
        var s = testPool.ResolveAll<TritonService>().Select(p => p.Factory).ToArray();
        Assert.IsTrue(s.Any(p => p is EfCoreTransFactory<TestDbContext>));
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
        Assert.IsTrue(s.Any(p => p is EfCoreTransFactory<TestDbContext>));
    }

    [Test]
    public void UseContext_with_static_options_creates_transactions()
    {
        ServicePool testPool = new();
        DbContextOptions options = new DbContextOptionsBuilder().Options;
        testPool.UseTriton().UseContext(typeof(ConfigurableContext), options);
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.IsTrue(s.Any(p => p.GetReadTransaction() is not null));
    }

    [Test]
    public void UseContext_with_config_method_creates_transactions()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext(typeof(ConfigurableContext), (DbContextOptionsBuilder _) => { });
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.IsTrue(s.Any(p => p.GetReadTransaction() is not null));
    }

    [Test]
    public void UseContext_generic_with_static_options_creates_transactions()
    {
        ServicePool testPool = new();
        DbContextOptions<ConfigurableContext> options = new DbContextOptionsBuilder<ConfigurableContext>().Options;
        testPool.UseTriton().UseContext<ConfigurableContext>(options);
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.IsTrue(s.Any(p => p.GetReadTransaction() is not null));
    }

    [Test]
    public void UseContext_generic_with_config_method_creates_transactions()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext((DbContextOptionsBuilder<ConfigurableContext> _) => { });
        var s = testPool.ResolveAll<TritonService>().ToArray();
        Assert.IsTrue(s.Any(p => p.GetReadTransaction() is not null));
    }

    [Test]
    public void UseMiddleware_with_out_parameter_registers_middleware()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseMiddleware<TestMiddleware>(out var m);
        Assert.IsFalse(m.PrologRan);
        testPool.Resolve<IMiddlewareRunner>()?.RunProlog(CrudAction.Commit, null);
        Assert.IsTrue(m.PrologRan);
        testPool.Resolve<IMiddlewareRunner>()?.RunEpilog(CrudAction.Commit, null);
        Assert.IsTrue(m.EpilogRan);
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
        Assert.IsFalse(actionRan);
        testPool.Resolve<IMiddlewareRunner>()?.RunProlog(CrudAction.Commit, null);
        Assert.IsTrue(actionRan);
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
        Assert.IsFalse(actionRan);
        testPool.Resolve<IMiddlewareRunner>()?.RunEpilog(CrudAction.Commit, null);
        Assert.IsTrue(actionRan);
    }

    [Test]
    public void UseTriton_with_config_callback()
    {
        ServicePool testPool = new();
        Assert.AreSame(testPool, testPool.UseTriton(p => Assert.IsTrue(p.GetType().Implements<ITritonConfigurable>())));
    }

    [Test]
    public void Multiple_UseTriton_calls_doesnt_duplicate_singletons()
    {
        ServicePool testPool = new();
        testPool.UseTriton();
        var tc = testPool.Resolve<ITritonConfigurable>();
        Assert.IsNotNull(tc);
        testPool.UseTriton(p => Assert.AreSame(tc, p));
        Assert.AreEqual(1, testPool.ResolveAll<ITritonConfigurable>().Count());
    }

    [Test]
    public void ResolveTritonService_resolves_service_for_context()
    {
        ServicePool testPool = new();
        testPool.UseTriton().UseContext<TestDbContext>();
        Assert.IsAssignableFrom<TritonService>(testPool.ResolveTritonService<TestDbContext>());
    }

    [Test]
    public void ResolveTritonService_initializes_all_required_singletons_if_none_registered()
    {
        ServicePool testPool = new();
        Assert.IsAssignableFrom<TritonService>(testPool.ResolveTritonService<TestDbContext>());
    }

    [Test]
    public void ConfigureMiddlewares_supports_callback()
    {
        new ServicePool().UseTriton().ConfigureMiddlewares(Assert.NotNull);
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