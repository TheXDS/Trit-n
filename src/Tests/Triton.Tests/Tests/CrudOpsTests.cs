#pragma warning disable CS1591

using NUnit.Framework;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests
{
    public class CrudOpsTests
    {
        private class DefaultImplServiceWrap : IService
        {
            private readonly Service svc;

            public DefaultImplServiceWrap(Service svc)
            {
                this.svc = svc;
            }

            public ICrudReadWriteTransaction GetTransaction()
            {
                return ((IService)svc).GetTransaction();
            }
        }

        private readonly Service _srv = new(new TestTransFactory());

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
            IService svc = new DefaultImplServiceWrap(_srv);
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
            using (var t = _srv.GetWriteTransaction())
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
            Assert.AreEqual(ServiceResult.Ok, t.CreateMany(new Models.Base.Model[] {
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
    }
}