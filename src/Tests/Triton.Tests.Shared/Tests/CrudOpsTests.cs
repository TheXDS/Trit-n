#pragma warning disable CS1591

using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests
{
    public partial class CrudOpsTests
    {
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
        public void SimpleReadTransactionTest()
        {
            using var t = _srv.GetReadTransaction();

            Post? post = t.Read<Post, long>(1L);
            Assert.IsInstanceOf<Post>(post);
            Assert.AreEqual("Test", post!.Title);

            Comment? comment = t.Read<Comment>(1L);
            Assert.IsInstanceOf<Comment>(comment);
            Assert.AreEqual("It works!", comment!.Content);
        }

        [Test]
        public async Task FullyAsyncReadTransactionTest()
        {
            await using var t = _srv.GetReadTransaction();

            Post? post = await t.ReadAsync<Post, long>(1L);
            Assert.IsInstanceOf<Post>(post);
            Assert.AreEqual("Test", post!.Title);

            Comment? comment = await t.ReadAsync<Comment>(1L);
            Assert.IsInstanceOf<Comment>(comment);
            Assert.AreEqual("It works!", comment!.Content);
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