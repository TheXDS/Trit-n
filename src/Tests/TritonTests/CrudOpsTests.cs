#pragma warning disable CS1591

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.Tests
{
    public class CrudOpsTests
    {
        private static readonly BlogService _srv = new BlogService();

        static CrudOpsTests()
        {
            using var c = new BlogContext();
            User u1, u2, u3;

            c.Users.AddRange(new[]
            {
                u1 = new User()
                {
                    Id = "user1",
                    PublicName = "User #1",
                    Joined = new DateTime(2001, 1, 1)
                },
                u2 = new User()
                {
                    Id = "user2",
                    PublicName = "User #2",
                    Joined = new DateTime(2009, 3, 4)
                },
                u3 = new User()
                {
                    Id = "user3",
                    PublicName = "User #3",
                    Joined = new DateTime(2004, 9, 11)
                }
            });

            c.Posts.Add(new Post()
            {
                Title = "Test",
                CreationTime = new DateTime(2016, 12, 31),
                Published = true,
                Content = "This is a test.",
                Author = u1,
                Comments =
                {
                    new Comment()
                    {
                        Author = u2,
                        Timestamp = new DateTime(2017,1,1),
                        Content = "It works!"
                    },
                    new Comment()
                    {
                        Author = u3,
                        Timestamp = new DateTime(2017,1,2),
                        Content = "Yay! c:"
                    },
                    new Comment()
                    {
                        Author = u1,
                        Timestamp = new DateTime(2017,1,1),
                        Content = "Shuddap >:("
                    },
                    new Comment()
                    {
                        Author = u3,
                        Timestamp = new DateTime(2017,1,1),
                        Content = "ok :c"
                    },

                }
            });

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
                var createResult = t.Create(new User()
                {
                    Id = "user4",
                    PublicName = "User 4"
                });

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


        [Test]
        public void CreateAndNotifyTest()
        {
            _srv.Configuration.Notifier.Reset();

            using var t = _srv.GetWriteTransaction();
            t.Create(new User()
            {
                Id = "user5",
                PublicName = "User #5"
            });
            t.Commit();

            Assert.IsTrue(_srv.Configuration.Notifier.Notified);
            Assert.IsInstanceOf<User>(_srv.Configuration.Notifier.Entity);
            Assert.AreEqual(CrudAction.Create, _srv.Configuration.Notifier.Action!.Value);
        }

        [Test]
        public void ReadAndNotifyTest()
        {
            _srv.Configuration.Notifier.Reset();

            using var t = _srv.GetReadTransaction();
            Post? post = t.Read<Post, long>(1L);

            Assert.IsTrue(_srv.Configuration.Notifier.Notified);
            Assert.IsInstanceOf<Post>(_srv.Configuration.Notifier.Entity);
            Assert.AreEqual(CrudAction.Read, _srv.Configuration.Notifier.Action!.Value);
        }

        [Test]
        public void UpdateAndNotifyTest()
        {
            _srv.Configuration.Notifier.Reset();


            User r;
            using (var t = _srv.GetReadTransaction())
            {
                r = t.Read<User, string>("user1").ReturnValue!;
            }

            r.PublicName = "Test #1!";

            using (var t = _srv.GetWriteTransaction())
            {
                Assert.True(t.Update(r).Success);
            }

            Assert.IsTrue(_srv.Configuration.Notifier.Notified);
            Assert.IsInstanceOf<User>(_srv.Configuration.Notifier.Entity);
            Assert.AreEqual(CrudAction.Update, _srv.Configuration.Notifier.Action!.Value);
        }

        [Test]
        public void DeleteAndNotifyTest()
        {
            _srv.Configuration.Notifier.Reset();

            using (var t = _srv.GetWriteTransaction())
            {
                Assert.True(t.Delete<Comment, long>(3L).Success);
            }

            Assert.IsTrue(_srv.Configuration.Notifier.Notified);
            Assert.IsInstanceOf<Comment>(_srv.Configuration.Notifier.Entity);
            Assert.AreEqual(CrudAction.Delete, _srv.Configuration.Notifier.Action!.Value);
        }


        private class BlogService : Service<BlogContext>
        {
            public IEnumerable<IGrouping<User, Post>> GetAllUsersFirst3Posts()
            {
                var t = GetReadTransaction();
                try
                {
                    return t.All<User>()
                        .Include(p => p.Posts)
                        .ThenInclude(p => p.Author)
                        .SelectMany(p => p.Posts.Take(3).OrderBy(q => q.CreationTime))
                        .ToList()
                        .GroupBy(p => p.Author);
                }
                finally
                {
                    t.Dispose();
                }
            }
            public TestConfiguration Configuration => (TestConfiguration)base.Configuration;

            public BlogService()
            {                
            }
        }

        private class TestConfiguration : ServiceConfiguration
        {
            public TestConfiguration()
            {                
            }

            public TestNotifier Notifier { get; } = new TestNotifier();
        }

        private class TestCrudTransFactory : ICrudTransactionFactory
        {
        }

        private class TestNotifier
        {
            public bool Notified { get; private set; }
            public Model? Entity { get; private set; }
            public CrudAction? Action { get; private set; }

            public void Reset()
            {
                Notified = false;
                Entity = null;
                Action = null;
            }

            public ServiceResult Notify(Model entity, CrudAction action)
            {
                Notified = true;
                Entity = entity;
                Action = action;
                return ServiceResult.Ok;
            }
        }
    }
}