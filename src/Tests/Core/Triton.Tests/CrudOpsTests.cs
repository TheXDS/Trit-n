#pragma warning disable CS1591

namespace TheXDS.Triton.Tests;
using NUnit.Framework;
using System.Threading.Tasks;
using MCART.Types.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;
using Models;
using Services;

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
            Assert.IsInstanceOf<ICrudReadTransaction>(t);
        }
        using (var t = _srv.GetWriteTransaction())
        {
            Assert.IsInstanceOf<ICrudWriteTransaction>(t);
        }
        using (var t = _srv.GetTransaction())
        {
            Assert.IsInstanceOf<ICrudReadWriteTransaction>(t);
        }
    }

    [Test]
    public void Service_defualt_impl_transaction_test()
    {
        ITritonService svc = new DefaultImplServiceWrap(_srv);
        using (var t = svc.GetReadTransaction())
        {
            Assert.IsInstanceOf<ICrudReadTransaction>(t);
        }
        using (var t = svc.GetWriteTransaction())
        {
            Assert.IsInstanceOf<ICrudWriteTransaction>(t);
        }
        using (var t = svc.GetTransaction())
        {
            Assert.IsInstanceOf<ICrudReadWriteTransaction>(t);
        }
    }

    [Test]
    public async Task GetAsyncTransactionTest()
    {
        await using (var t = _srv.GetReadTransaction())
        {
            Assert.IsInstanceOf<ICrudReadTransaction>(t);
        }
        await using (var t = _srv.GetWriteTransaction())
        {
            Assert.IsInstanceOf<ICrudWriteTransaction>(t);
        }
        await using (var t = _srv.GetTransaction())
        {
            Assert.IsInstanceOf<ICrudReadWriteTransaction>(t);
        }
    }

    [Test]
    public void TransactionDisposalTest()
    {
        IDisposableEx t;

        using (t = _srv.GetReadTransaction())
        {
            Assert.False(t.IsDisposed);
        }
        Assert.True(t.IsDisposed);

        using (t = _srv.GetWriteTransaction())
        {
            Assert.False(t.IsDisposed);
        }
        Assert.True(t.IsDisposed);

        using (t = _srv.GetTransaction())
        {
            Assert.False(t.IsDisposed);
        }
        Assert.True(t.IsDisposed);
    }

    [Test]
    public async Task CreateAndVerifyTransactionTest()
    {
        await using (var t = _srv.GetWriteTransaction())
        {
            var createResult = t.Create(new User("user4", "User 4"));

            Assert.IsTrue(createResult.Success);
            Assert.IsNull(createResult.Reason);
        }

        // Realizar prueba post-disposal para comprobar correctamente el guardado.

        await using (var t = _srv.GetReadTransaction())
        {
            var readResult = t.Read<User, string>("user4", out var u);

            Assert.IsTrue(readResult.Success);
            Assert.IsNull(readResult.Reason);
            Assert.IsInstanceOf<User>(u);
            Assert.AreEqual("User 4", u!.PublicName);
        }
    }

    [Test]
    public void CreateMany_test()
    {
        using var t = _srv.GetWriteTransaction();
        Assert.AreEqual(ServiceResult.Ok, t.CreateMany(new Model[] {
            new User("user7", "User #7"),
            new User("user8", "User #8"),
            new User("user9", "User #9"),
        }));
    }

    [Test]
    public void UpdateAndVerifyTransactionTest()
    {
        User r;
        using (var t = _srv.GetReadTransaction())
        {
            r = t.Read<User, string>("user1").ReturnValue!;
        }

        r.PublicName = "Test #1";

        using (var t = _srv.GetWriteTransaction())
        {
            Assert.True(t.Update(r).Success);
        }

        using (var t = _srv.GetReadTransaction())
        {
            r = t.Read<User, string>("user1").ReturnValue!;
        }
        Assert.AreEqual("Test #1", r.PublicName);
    }

    [Test]
    public void DeleteAndVerifyTransactionTest()
    {
        using (var t = _srv.GetWriteTransaction())
        {
            Assert.IsTrue(t.Delete<User, string>("user3").Success);
        }
        using (var t = _srv.GetReadTransaction())
        {
            Assert.IsNull(t.Read<User, string>("user3").ReturnValue);
        }
    }

    public async Task ReadAsync_Test()
    {
        await using var t = _srv.GetReadTransaction();
        User r = (await t.ReadAsync<User, string>("user1")).ReturnValue!;
        Assert.IsNotNull(r);
        Assert.IsAssignableFrom<User>(r);
    }
}