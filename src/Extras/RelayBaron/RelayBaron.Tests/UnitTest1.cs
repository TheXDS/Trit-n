using NUnit.Framework;
using RelayBaron.Client;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests;
using TheXDS.Triton.Models;
using System.Linq;
using TheXDS.MCART.Types.Extensions;

namespace RelayBaron.Tests
{
    public class ServerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InstanceTest()
        {
            var srv = new Server.RelayBaronProtocol().BuildServer(65000);
            srv.Start();
            Assert.True(srv.IsAlive);
            srv.Stop();
        }
    }

    public class ClientTests
    {
        private static readonly BlogService srv = new BlogService();

        [Test]
        public void RegisterActionsTest()
        {
            void CreateNewPost(string id)
            {
                using var t = srv.GetReadWriteTransaction();
                var u = t.Read<User, string>(id).ReturnValue!;
                t.Create(
                    new Post(
                        $"Hello, I'm {u.PublicName}",
                        $"Hello everyone. I'm new here, my name is {u.PublicName}.")
                    { 
                        Author = u
                    });
                t.Commit();
            }

            void CreateFirstComment(long id)
            {
                using var t = srv.GetReadWriteTransaction();
                var u = t.Read<Post, long>(id).ReturnValue!;
                if (!u.Comments.Any())
                {
                    u.Comments.Add(new Comment("FIRST!!!") { Author = u.Author });
                }
                t.Commit();
            }

            var notifier = new BaronNotifier();
            notifier.Register<User>(CrudAction.Create, CreateNewPost);
            notifier.Register<Post, long>(CrudAction.Create, CreateFirstComment);
        }
    }
}