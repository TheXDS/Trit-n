using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading.Tasks;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services.Base;
using TheXDS.Triton.Services;
using System.Linq;
using TheXDS.Triton.TestModels;

namespace TheXDS.Triton.Tests
{
    public class Tests
    {
        /* COSAS A PROBAR
         * ==============
         * - Carga de AppDomain en Net Core 3
         *
         */

        [SetUp]
        public void Setup()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [Test]
        public void AppDomainCreationTest()
        {
            const string p = @"C:\Users\xds_x\source\repos\TheXDS\MCART\Build\bin\Consoleer\Debug\netcoreapp3.0" + @"\Consoleer.dll";
            var x = MCART.Resources.RtInfo.RtSupport(this.GetType().Assembly);
            var alc = new TestLoadContext(p);
            var asm = new WeakReference(alc.LoadFromAssemblyPath(p), false);
            Assert.True(asm.IsAlive);
            Assert.NotNull(asm.Target);
            ((Assembly)asm.Target).EntryPoint.Invoke(null, new object[] { new string[] { "--Detail:alot" } });
            alc.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private class TestLoadContext: AssemblyLoadContext
        {
            private AssemblyDependencyResolver _resolver;

            public TestLoadContext(string path) : base(true)
            {
                _resolver = new AssemblyDependencyResolver(path);
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                return (_resolver.ResolveAssemblyToPath(assemblyName) is string path)
                    ? LoadFromAssemblyPath(path)
                    : null;
            }
        }
    }

    public class LibOpsTests
    {
        private static readonly TestConfiguration _testConfig = new TestConfiguration();
        private static readonly IService _srv = new Service<BlogContext>(_testConfig);

        static LibOpsTests()
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
            using (var t = _srv.GetFullTransaction())
            {
                Assert.IsInstanceOf<ICrudFullTransaction>(t);
            }
        }

        [Test]
        public void CreateAndVeryfyTransactionTest()
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
            using var t = _srv.GetFullTransaction();
            Post? post = t.Read<Post, long>(1L);
            Assert.IsInstanceOf<Post>(post);
            Assert.AreEqual("Test", post!.Title);
        }

        [Test]
        public void UpdateAndVeryfyTransactionTest()
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
            _testConfig.Notifier.Reset();

            using var t = _srv.GetWriteTransaction();         
            var createResult = t.Create(new User()
            {
                Id = "user5",
                PublicName = "User #5"
            });

            Assert.IsTrue(_testConfig.Notifier.Notified);
            Assert.IsInstanceOf<User>(_testConfig.Notifier.Entity);
            Assert.AreEqual(CrudAction.Create, _testConfig.Notifier.Action!.Value);
        }









        [Test]
        public void RelatedDataEagerLoadingTest()
        {
            using var c = new BlogContext();

            var result = c.Users
                .Include(p => p.Posts)
                .ThenInclude(p => p.Author)
                .SelectMany(p => p.Posts.Take(3).OrderBy(q => q.CreationTime))
                .ToList()
                .GroupBy(p => p.Author);

        }





        private class TestConfiguration : IServiceConfiguration
        {
            public ICrudTransactionFactory CrudTransactionFactory { get; } = new TestCrudTransFactory();

            public TestNotifier Notifier { get; } = new TestNotifier();

            ICrudNotificationSource? IConnectionConfiguration.Notifier => Notifier;
        }
        private class TestCrudTransFactory : ICrudTransactionFactory
        {
            public ICrudFullTransaction ManufactureTransaction<T>(IConnectionConfiguration configuration) where T : DbContext, new()
            {
                return new CrudTransaction<T>(configuration);
            }
        }
        private class TestNotifier : ICrudNotificationSource
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