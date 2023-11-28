#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests;

public class TransactionConfigurationTests
{
    [Test]
    public void AddFirstProlog_Test()
    {
        bool ranFirst = false;
        bool ranLast = false;
        ServiceResult? FirstAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            ranFirst = !ranLast;
            return null;
        }
        ServiceResult? DummyAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            ranLast = !ranFirst;
            return null;
        }
        TransactionConfiguration c = new();
        c.AddProlog(DummyAction);
        c.AddFirstProlog(FirstAction);
        c.RunProlog(CrudAction.Commit, null);
        
        Assert.That(ranFirst);
        Assert.That(ranLast, Is.False);
    }
    
    [Test]
    public void AddFirstEpilog_Test()
    {
        bool ranFirst = false;
        bool ranLast = false;
        ServiceResult? FirstAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            ranFirst = !ranLast;
            return null;
        }
        ServiceResult? DummyAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            ranLast = !ranFirst;
            return null;
        }
        TransactionConfiguration c = new();
        c.AddEpilog(DummyAction);
        c.AddFirstEpilog(FirstAction);
        c.RunEpilog(CrudAction.Commit, null);
        
        Assert.That(ranFirst);
        Assert.That(ranLast, Is.False);
    }

    private class Flaggy1TestMiddleware : ITransactionMiddleware
    {
        ServiceResult? ITransactionMiddleware.PrologAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            _test_AttachAt_flag1 = !_test_AttachAt_flag2;
            return null;
        }
        ServiceResult? ITransactionMiddleware.EpilogAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            _test_AttachAt_flag1 = !_test_AttachAt_flag2;
            return null;
        }
    }
    
    private class Flaggy2TestMiddleware : ITransactionMiddleware
    {
        ServiceResult? ITransactionMiddleware.PrologAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            _test_AttachAt_flag2 = !_test_AttachAt_flag1;
            return null;
        }
        ServiceResult? ITransactionMiddleware.EpilogAction(CrudAction crudAction, IEnumerable<Model>? entity)
        {
            _test_AttachAt_flag2 = !_test_AttachAt_flag1;
            return null;
        }
    }
    
    [Test]
    public void AttachAt_Test()
    {
        TransactionConfiguration c = new();
        c.Attach<Flaggy1TestMiddleware>();
        c.AttachAt<Flaggy2TestMiddleware>(ActionPosition.Early, ActionPosition.Late);
        
        _test_AttachAt_flag1 = false;
        _test_AttachAt_flag2 = false;
        c.RunProlog(CrudAction.Commit, null);
        Assert.That(_test_AttachAt_flag2);
        Assert.That(_test_AttachAt_flag1, Is.False);
        
        _test_AttachAt_flag1 = false;
        _test_AttachAt_flag2 = false;
        c.RunEpilog(CrudAction.Commit, null);
        Assert.That(_test_AttachAt_flag1);
        Assert.That(_test_AttachAt_flag2, Is.False);
    }

    private static bool _test_AttachAt_flag1 = false;
    private static bool _test_AttachAt_flag2 = false;
}