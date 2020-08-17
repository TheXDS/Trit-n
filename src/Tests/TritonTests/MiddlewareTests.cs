#pragma warning disable CS1591

using NUnit.Framework;
using System.Linq;
using TheXDS.Triton.Models;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests
{
    public class MiddlewareTests
    {
        private static readonly BlogService _srv = new BlogService();

        [Test]
        public void RunMiddlewareTest()
        {
            bool prologDidRun = false, epilogDidRun = false;

            ServiceResult? TestProlog(CrudAction arg1, Model? arg2)
            {
                if (prologDidRun) return null;
                Assert.AreEqual(CrudAction.Create, arg1);
                Assert.IsInstanceOf<Post>(arg2);
                Assert.AreEqual("0", arg2!.IdAsString);
                prologDidRun = true;
                return null;
            }

            ServiceResult? TestEpilog(CrudAction arg1, Model? arg2)
            {
                if (epilogDidRun) return null;
                Assert.AreEqual(CrudAction.Create, arg1);
                Assert.IsInstanceOf<Post>(arg2);
                Assert.AreNotEqual("0", arg2!.IdAsString);
                epilogDidRun = true;
                return null;
            }

            using var j = _srv.GetReadWriteTransaction();

            var u = j.All<User>().First();

            _srv.Configuration.TransactionConfiguration.AddProlog(TestProlog);
            _srv.Configuration.TransactionConfiguration.AddEpilog(TestEpilog);
            
            Assert.False(prologDidRun);
            Assert.False(epilogDidRun);
            j.Create(new Post("Test", "Middleware test!", u));
            Assert.True(prologDidRun);
            Assert.True(epilogDidRun);

            Assert.True(j.Commit().Success);
        }
    }
}