#pragma warning disable CS1591

using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests
{
    [SetUpFixture]
    public class DbPopulation
    {
        [OneTimeSetUp]
        public void PopulateDb()
        {
            using var t = new Service(new InMemoryTransFactory()).GetTransaction();
            if (!t.All<User>().Any())
            {
                User u1, u2, u3;
                Post post;

                t.CreateMany(
                    u1 = new("user1", "User #1", new DateTime(2001, 1, 1)),
                    u2 = new("user2", "User #2", new DateTime(2009, 3, 4)),
                    u3 = new("user3", "User #3", new DateTime(2004, 9, 11))
                );
                t.Create(post = new Post("Test", "This is a test.", u1, new DateTime(2016, 12, 31)) { Published = true, Id = 1L });
                t.CreateMany(
                    new Comment(u2, post, "It works!", new DateTime(2017, 1, 1)) { Id = 1L },
                    new Comment(u3, post, "Yay! c:", new DateTime(2017, 1, 2)) { Id = 2L },
                    new Comment(u1, post, "Shuddap >:(", new DateTime(2017, 1, 3)) { Id = 3L },
                    new Comment(u3, post, "ok :c", new DateTime(2017, 1, 4)) { Id = 4L }
                );
            }
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            InMemoryCrudTransaction.Wipe();
        }
    }

    public class CrudOpsTests
    {
        private readonly Service _srv = new Service(new InMemoryTransFactory());

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

        //[Test]
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

        //[Test]
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