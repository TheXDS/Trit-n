#pragma warning disable CS1591

using NUnit.Framework;
using System.Linq;
using TheXDS.Triton.InMemory.Services;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests
{
    public partial class MiddlewareTests
    {
        public class DefaultMiddleware : ITransactionMiddleware { }

        [Theory]
        public void ITransactionMiddleware_has_default_implememtations(CrudAction action)
        {
            ITransactionMiddleware transaction = new DefaultMiddleware();
            Assert.Null(transaction.PrologAction(action, new User("x", "Test"))); 
            Assert.Null(transaction.EpilogAction(action, new User("x", "Test")));
        }

        [Test]
        public void Run_Middleware_test()
        {
            Service _srv = new(new TestTransFactory());
            bool prologDidRun = false, epilogDidRun = false;

            ServiceResult? TestProlog(CrudAction arg1, Model? arg2)
            {
                if (!prologDidRun)
                {
                    Assert.AreEqual(CrudAction.Create, arg1);
                    Assert.IsInstanceOf<Post>(arg2);
                    Assert.AreEqual("0", arg2!.IdAsString);
                    prologDidRun = true;
                }
                return null;
            }

            ServiceResult? TestEpilog(CrudAction arg1, Model? arg2)
            {
                if (!epilogDidRun)
                {
                    Assert.True(prologDidRun);
                    Assert.AreEqual(CrudAction.Create, arg1);
                    Assert.IsInstanceOf<Post>(arg2);
                    epilogDidRun = true;
                }
                return null;
            }

            using var j = _srv.GetTransaction();

            var u = j.All<User>().First();

            _srv.Configuration.AddProlog(TestProlog);
            _srv.Configuration.AddEpilog(TestEpilog);
            
            Assert.False(prologDidRun);
            Assert.False(epilogDidRun);
            Assert.True(j.Create(new Post("Test", "Middleware test!", u)).Success);
            Assert.True(prologDidRun);
            Assert.True(epilogDidRun);            
            Assert.True(j.Commit().Success);
        }
    }
}