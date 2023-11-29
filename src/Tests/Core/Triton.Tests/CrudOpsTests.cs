#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;
using TheXDS.Triton.Tests.Models;
using TheXDS.Triton.Tests.Services;

namespace TheXDS.Triton.Tests;

public class CrudOpsTests
{
    private class DefaultImplServiceWrap : ITritonService
    {
        private readonly TritonService _svc;

        public DefaultImplServiceWrap(TritonService svc)
        {
            _svc = svc;
        }

        public ICrudReadWriteTransaction GetTransaction()
        {
            return ((ITritonService)_svc).GetTransaction();
        }
    }

    private readonly TritonService _srv = new(new TestTransFactory());

    [Test]
    public void GetTransactionTest()
    {
        using (var t = _srv.GetReadTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudReadTransaction>());
        }
        using (var t = _srv.GetWriteTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudWriteTransaction>());
        }
        using (var t = _srv.GetTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudReadWriteTransaction>());
        }
    }

    [Test]
    public void Service_defualt_impl_transaction_test()
    {
        ITritonService svc = new DefaultImplServiceWrap(_srv);
        using (var t = svc.GetReadTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudReadTransaction>());
        }
        using (var t = svc.GetWriteTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudWriteTransaction>());
        }
        using (var t = svc.GetTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudReadWriteTransaction>());
        }
    }

    [Test]
    public async Task GetAsyncTransactionTest()
    {
        await using (var t = _srv.GetReadTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudReadTransaction>());
        }
        await using (var t = _srv.GetWriteTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudWriteTransaction>());
        }
        await using (var t = _srv.GetTransaction())
        {
            Assert.That(t, Is.InstanceOf<ICrudReadWriteTransaction>());
        }
    }

    [Test]
    public void TransactionDisposalTest()
    {
        IDisposableEx t;

        using (t = _srv.GetReadTransaction())
        {
            Assert.That(t.IsDisposed, Is.False);
        }
        Assert.That(t.IsDisposed);

        using (t = _srv.GetWriteTransaction())
        {
            Assert.That(t.IsDisposed, Is.False);
        }
        Assert.That(t.IsDisposed);

        using (t = _srv.GetTransaction())
        {
            Assert.That(t.IsDisposed, Is.False);
        }
        Assert.That(t.IsDisposed);
    }

    [Test]
    public async Task CreateAndVerifyTransactionTest()
    {
        await using (var t = _srv.GetWriteTransaction())
        {
            var createResult = t.Create(new User("user4", "User 4"));

            Assert.That(createResult.Success);
            Assert.That(createResult.Reason, Is.Null);
        }

        // Realizar prueba post-disposal para comprobar correctamente el guardado.

        await using (var t = _srv.GetReadTransaction())
        {
            var readResult = t.Read<User, string>("user4", out var u);

            Assert.That(readResult.Success);
            Assert.That(readResult.Reason, Is.Null);
            Assert.That(u, Is.InstanceOf<User>());
            Assert.That(u!.PublicName, Is.EqualTo("User 4"));
        }
    }

    [Test]
    public void CreateMany_test()
    {
        using var t = _srv.GetWriteTransaction();
        Assert.That(t.Create(new Model[] {
            new User("user7", "User #7"),
            new User("user8", "User #8"),
            new User("user9", "User #9"),
        }), Is.EqualTo(ServiceResult.Ok));
    }

    [Test]
    public void UpdateAndVerifyTransactionTest()
    {
        User r;
        using (var t = _srv.GetReadTransaction())
        {
            r = t.Read<User, string>("user1").Result!;
        }

        r.PublicName = "Test #1";

        using (var t = _srv.GetWriteTransaction())
        {
            Assert.That(t.Update(r).Success);
        }

        using (var t = _srv.GetReadTransaction())
        {
            r = t.Read<User, string>("user1").Result!;
        }
        Assert.That(r.PublicName, Is.EqualTo("Test #1"));
    }

    [Test]
    public void Delete_and_verify_transaction_test()
    {
        using (var t = _srv.GetWriteTransaction())
        {
            Assert.That(t.Delete<User, string>("user3").Success);
        }
        using (var t = _srv.GetReadTransaction())
        {
            Assert.That(t.Read<User, string>("user3").Result, Is.Null);
        }
    }

    [Test]
    public async Task ReadAsync_Test()
    {
        await using var t = _srv.GetReadTransaction();
        User r = (await t.ReadAsync<User, string>("user1")).Result!;
        Assert.That(r, Is.Not.Null);
        Assert.That(r, Is.InstanceOf<User>());
    }

    [Test]
    public async Task SearchAsync_test()
    {
        await using var t = _srv.GetReadTransaction();
        var r = (await t.SearchAsync<User>(p => p.PublicName != null)).Result!;
        Assert.That(r, Is.Not.Null);
        Assert.That(r.Length, Is.Not.Zero);
    }
}