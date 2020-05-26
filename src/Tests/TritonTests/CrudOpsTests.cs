#pragma warning disable CS1591

using NUnit.Framework;
using System;
using System.Linq;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Tests
{
    public partial class CrudOpsTests
    {
        private static readonly BlogService _srv = new BlogService();

        static CrudOpsTests()
        {
            using var c = new BlogContext();

            User u1, u2, u3;
            Post post;

            c.Users.AddRange(
                u1 = new User("user1", "User #1") { Joined = new DateTime(2001, 1, 1) },
                u2 = new User("user2", "User #2") { Joined = new DateTime(2009, 3, 4) },
                u3 = new User("user3", "User #3") { Joined = new DateTime(2004, 9, 11) }
            );

            c.Posts.Add(post = new Post("Test", "This is a test.", u1)
            {
                CreationTime = new DateTime(2016, 12, 31),
                Published = true,
            });

            c.Comments.AddRange(
                new Comment(u2, post, "It works!") { Timestamp = new DateTime(2017, 1, 1) },
                new Comment(u3, post, "Yay! c:") { Timestamp = new DateTime(2017, 1, 2) },
                new Comment(u1, post, "Shuddap >:(") { Timestamp = new DateTime(2017, 1, 3) },
                new Comment(u3, post, "ok :c") { Timestamp = new DateTime(2017, 1, 4) }
            );

            c.SaveChanges();
        }

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
            using (var t = _srv.GetReadWriteTransaction())
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

            using (t = _srv.GetReadWriteTransaction())
            {
                Assert.False(t.IsDisposed);
            }
            Assert.True(t.IsDisposed);
        }

        [Test]
        public void CreateAndVerifyTransactionTest()
        {
            using (var t = _srv.GetWriteTransaction())
            {
                var createResult = t.Create(new User("user4", "User 4"));
                Assert.IsTrue(createResult.Success);
                Assert.IsNull(createResult.Reason);
            }
            using (var t = _srv.GetReadTransaction())
            {
                var readResult = t.Read<User, string>("user4", out var u);

                Assert.IsTrue(readResult.Success);
                Assert.IsNull(readResult.Reason);
                Assert.IsInstanceOf<User>(u);
                Assert.AreEqual("User 4", u!.PublicName);
            }
        }

        [Test]
        public void ReadTransactionTest()
        {
            using var t = _srv.GetReadWriteTransaction();

            Post? post = t.Read<Post, long>(1L);
            Assert.IsInstanceOf<Post>(post);
            Assert.AreEqual("Test", post!.Title);

            Comment? comment = t.Read<Comment>(1L);
            Assert.IsInstanceOf<Comment>(comment);
            Assert.AreEqual("It works!", comment!.Content);
        }

        [Test]
        public void FailToReadTest()
        {
            using var t = _srv.GetReadWriteTransaction();
            var post = t.Read<Post, long>(999L);
            Assert.IsFalse(post.Success);
            Assert.AreEqual(FailureReason.NotFound, post.Reason);
            Assert.IsNull(post.ReturnValue);
        }

        [Test]
        public void RelatedDataEagerLoadingTest()
        {
            var q = _srv.GetAllUsersFirst3Posts().ToList();

            /* -= Según la base de datos de prueba: =-
             * Existen 3 usuarios, y únicamente el primer usuario debe tener un
             * Post. Los demás usuarios no deben tener ninguno.
             * 
             * Por la forma en que el Query está construido, solo se debe
             * obtener al primer usuario y a su correspondiente post.
             */

            Assert.AreEqual(1, q.Count);
            Assert.AreEqual("user1", q[0].Key.Id);
            Assert.AreEqual(1, q[0].Count());
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