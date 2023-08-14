#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;
using TheXDS.Triton.Tests.Services;

namespace TheXDS.Triton.Tests;

public partial class MiddlewareTests
{
    public class DefaultMiddleware : ITransactionMiddleware { }

    [Theory]
    public void ITransactionMiddleware_has_default_implememtations(CrudAction action)
    {
        ITransactionMiddleware transaction = new DefaultMiddleware();
        Model[] entities = { new User("x", "Test") };
        Assert.That(transaction.PrologAction(action, entities), Is.Null); 
        Assert.That(transaction.EpilogAction(action, entities), Is.Null);
    }

    [Test]
    public void Run_Middleware_test()
    {
        TritonService _srv = new(new TestTransFactory());
        bool prologDidRun = false, epilogDidRun = false;

        ServiceResult? TestProlog(CrudAction arg1, IEnumerable<Model>? arg2)
        {
            if (!prologDidRun)
            {
                Assert.AreEqual(CrudAction.Create, arg1);
                Assert.That(arg2, Is.Not.Null);
                var j = arg2!.First();
                Assert.IsInstanceOf<Post>(j);
                Assert.AreEqual("0", j!.IdAsString);
                prologDidRun = true;
            }
            return null;
        }

        ServiceResult? TestEpilog(CrudAction arg1, IEnumerable<Model>? arg2)
        {
            if (!epilogDidRun)
            {
                Assert.True(prologDidRun);
                Assert.AreEqual(CrudAction.Create, arg1);
                Assert.That(arg2, Is.Not.Null);
                var j = arg2!.First();
                Assert.IsInstanceOf<Post>(j);
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