#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Middleware;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests;

public abstract class TransactionMiddlewareExecutionTests<T> where T : ITransactionFactory, new()
{
    [Test]
    public void Commit_operation_executes_Middleware()
    {
        new MiddlewareRunCheck()
        {
            ExpectedAction = CrudAction.Commit,
        }.ExecuteTest(t => Assert.That(t.Commit().Success, Is.True));
    }

    [Test]
    public void Create_operation_executes_Middleware()
    {
        var g = Guid.NewGuid().ToString();
        new MiddlewareRunCheck()
        {
            ExpectedAction = CrudAction.Create,
            ExpectedPrologModelType = typeof(User),
            ExpectedEpilogModelType = typeof(User),
            ExtraEpilogAssertions = m => Assert.That(((User)m[0]).Id, Is.EqualTo(g))
        }.ExecuteTest(t => Assert.That(t.Create(new User(g, g)).Success, Is.True));
    }

    [Test]
    public void Read_operation_executes_Middleware()
    {
        var g = Guid.NewGuid().ToString();
        User? u = null;
        new MiddlewareRunCheck()
        {
            ExpectedAction = CrudAction.Read,
            ExpectedPrologModelType = typeof(User),
            ExpectedEpilogModelType = typeof(User),
            ExtraPrologAssertions = m => Assert.That(((User)m[0]).Id, Is.EqualTo(g)),
            ExtraEpilogAssertions = m => Assert.That(((User)m[0]).Id, Is.EqualTo(g))
        }.ExecuteTest(
            t => _ = t.Create(u = new(g, "user")),
            t => _ = t.Read<User, string>(g));
    }

    [Test]
    public void Update_operation_executes_Middleware()
    {
        var g = Guid.NewGuid().ToString();
        User? u = null;
        new MiddlewareRunCheck()
        {
            ExpectedAction = CrudAction.Update,
            ExpectedPrologModelType = typeof(User),
            ExpectedEpilogModelType = typeof(User),
            ExtraPrologAssertions = m => Assert.That(((User)m[0]).PublicName, Is.EqualTo("user")),
            ExtraEpilogAssertions = m => Assert.That(((User)m[0]).PublicName, Is.EqualTo(g))
        }.ExecuteTest(
            t => _ = t.Create(u = new(g, "user")),
            t =>
            {
                var newU = new User(g, g);
                t.Update(newU);
            });
    }

    [Test]
    public void Delete_operation_executes_Middleware()
    {
        var g = Guid.NewGuid().ToString();
        User? u = null;
        new MiddlewareRunCheck()
        {
            ExpectedAction = CrudAction.Delete,
            ExpectedPrologModelType = typeof(User),
            ExtraPrologAssertions = m => Assert.That(((User)m[0]).Id, Is.EqualTo(g)),
            ExpectedEpilogModelType = typeof(User),
            ExtraEpilogAssertions = m => Assert.That(((User)m[0]).Id, Is.EqualTo(g))
        }.ExecuteTest(
            t => _ = t.Create(u = new(g, "user")),
            t => t.Delete<User, string>(g));
    }

    private class MiddlewareRunCheck : ITransactionMiddleware
    {
        public void ExecuteTest(Action<ICrudReadWriteTransaction> callback) => ExecuteTest(null, callback);

        public void ExecuteTest(Action<ICrudReadWriteTransaction>? setupCallback, Action<ICrudReadWriteTransaction> callback)
        {
            var transactionConfig = new TransactionConfiguration();
            TritonService service = new(transactionConfig, new T());
            if (setupCallback is not null)
            {
                using var setupTransaction = service.GetTransaction();
                setupCallback?.Invoke(setupTransaction);
                setupTransaction.Commit();
            }
            transactionConfig.Attach(this);
            using var transaction = service.GetTransaction();
            Assert.That(PrologRan, Is.False);
            Assert.That(EpilogRan, Is.False);
            callback.Invoke(transaction);
            Assert.That(PrologRan, Is.True);
            Assert.That(EpilogRan, Is.True);
        }

        public CrudAction ExpectedAction { get; init; }

        public Type? ExpectedPrologModelType { get; init; }

        public Action<Model[]>? ExtraPrologAssertions { get; init; }

        public Type? ExpectedEpilogModelType { get; init; }

        public Action<Model[]>? ExtraEpilogAssertions { get; init; }

        public bool PrologRan { get; private set; }

        public bool EpilogRan { get; private set; }

        ServiceResult? ITransactionMiddleware.PrologAction(CrudAction action, IEnumerable<Model>? entities)
        {
            if (!PrologRan)
            {
                Assert.That(EpilogRan, Is.False);
                Assert.That(action, Is.EqualTo(ExpectedAction));
                if (ExpectedPrologModelType is null)
                {
                    Assert.That(entities, Is.Null);
                }
                else
                {
                    Assert.That(entities, Is.Not.Null);
                    Assert.That(entities!.FirstOrDefault(), Is.InstanceOf(ExpectedPrologModelType));
                    ExtraPrologAssertions?.Invoke(entities.NotNull().ToArray());
                }
                PrologRan = true;
            }
            return null;
        }

        ServiceResult? ITransactionMiddleware.EpilogAction(CrudAction action, IEnumerable<Model>? entities)
        {
            if (!EpilogRan)
            {
                Assert.That(PrologRan, Is.True);
                Assert.That(action, Is.EqualTo(ExpectedAction));
                if (ExpectedEpilogModelType is null)
                {
                    Assert.That(entities, Is.Null);
                }
                else
                {
                    Assert.That(entities, Is.Not.Null);
                    Assert.That(entities!.FirstOrDefault(), Is.InstanceOf(ExpectedEpilogModelType));
                    ExtraEpilogAssertions?.Invoke(entities.NotNull().ToArray());
                }
                EpilogRan = true;
            }
            return null;
        }
    }
}