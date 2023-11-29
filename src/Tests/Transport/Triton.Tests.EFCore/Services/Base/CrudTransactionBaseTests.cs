#pragma warning disable 1591

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;
using TheXDS.Triton.Services.Base;
using TheXDS.Triton.Tests.EFCore.Models;

namespace TheXDS.Triton.Tests.EFCore.Services.Base;

public class CrudTransactionBaseTests
{
    private class TestClass : CrudTransactionBase<BlogContext>
    {
        public TestClass() : base(new TransactionConfiguration(), (DbContextOptions?)null) { }

        public IMiddlewareConfigurator Configurator => (IMiddlewareConfigurator)_configuration;

        public static ServiceResult Test_ResultFromException(Exception ex) => ResultFromException(ex);

        public static CrudAction Test_Map(EntityState state) => Map(state);

        public ServiceResult? Test_TryCall_void(CrudAction action, Delegate op, params object?[] args)
        {
            return TryCall(action, op, args);
        }

        public ServiceResult? Test_TryCall<T>(CrudAction action, Delegate op, out T returnValue, params object?[] args)
        {
            return TryCall(action, op, out returnValue, args);
        }
        public ServiceResult<T?>? Test_TryCall<T>(CrudAction action, Delegate op, params object?[] args)
        {
            return TryCall<T>(action, op, args);
        }
    }

    [Test]
    public void ResultFromException_contract_test()
    {
        Assert.Throws<ArgumentNullException>(()=>TestClass.Test_ResultFromException(null!));
    }

    [TestCase(typeof(NullReferenceException), FailureReason.NotFound)]
    [TestCase(typeof(TaskCanceledException), FailureReason.NetworkFailure)]
    [TestCase(typeof(DbUpdateConcurrencyException), FailureReason.ConcurrencyFailure)]
    [TestCase(typeof(DbUpdateException), FailureReason.DbFailure)]
    [TestCase(typeof(RetryLimitExceededException), FailureReason.NetworkFailure)]
    public void ResultFromException_with_common_failures_test(Type exType, FailureReason reason)
    {
        var result = TestClass.Test_ResultFromException(exType.New<Exception>());
        Assert.That(result.Reason, Is.EqualTo(reason));
    }

    [TestCase(typeof(ArgumentOutOfRangeException))]
    [TestCase(typeof(StackOverflowException))]
    [TestCase(typeof(TypeLoadException))]
    public void ResultFromException_with_unexpected_exception_test(Type exType)
    {
        var ex = exType.New<Exception>();
        var result = TestClass.Test_ResultFromException(ex);
        Assert.That(result.Reason, Is.Not.Null);
        Assert.That((int)result.Reason!, Is.EqualTo(ex.HResult));
    }

    [TestCase(EntityState.Added, CrudAction.Create)]
    [TestCase(EntityState.Modified, CrudAction.Update)]
    [TestCase(EntityState.Deleted, CrudAction.Delete)]
    public void Map_with_known_states_test(EntityState state, CrudAction expected)
    {
        Assert.That(TestClass.Test_Map(state), Is.EqualTo(expected));
    }

    [Test]
    public void Map_with_unknown_states_test()
    {
        Assert.That(TestClass.Test_Map(EntityState.Detached), Is.EqualTo(CrudAction.Read));
        Assert.That(TestClass.Test_Map(EntityState.Unchanged), Is.EqualTo(CrudAction.Read));
    }

    [Test]
    public void TryCall_with_void_method_delegate_test()
    {
        bool delegateRan = false;
        var test = new TestClass();

        void TestDelegate(bool arg) => delegateRan = arg;

        var result = test.Test_TryCall_void(CrudAction.Create, TestDelegate, true);
        Assert.That(result, Is.Null);
        Assert.That(delegateRan, Is.Not.Null);
    }

    [Test]
    public void TryCall_with_value_returning_method_delegate_test()
    {
        bool delegateRan = false;
        var test = new TestClass();

        int TestDelegate(bool arg)
        {
            delegateRan = arg;
            return 1;
        }

        var result = test.Test_TryCall(CrudAction.Create, TestDelegate, out int returnValue, true);

        Assert.That(result, Is.Null);
        Assert.That(delegateRan, Is.True);
        Assert.That(returnValue, Is.EqualTo(1));
    }

    [Test]
    public void TryCall_with_invalid_return_type_method_delegate_test()
    {
        bool delegateRan = false;
        var test = new TestClass();

        int TestDelegate(bool arg)
        {
            delegateRan = arg;
            return 1;
        }

        Assert.Throws<InvalidCastException>(() => _ = test.Test_TryCall(CrudAction.Create, TestDelegate, out string returnValue, true));
        Assert.That(delegateRan, Is.False);
    }

    [Test]
    public void TryCall_exception_handling_test()
    {
        var test = new TestClass();

        static int TestDelegate(bool arg) => throw new Exception() { HResult = 0xdead };

        var result = test.Test_TryCall(CrudAction.Create, TestDelegate, out int returnValue, true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(returnValue, Is.EqualTo(default(int)));
        Assert.That((int)result.Reason!, Is.EqualTo(0xdead));
    }

    [Test]
    public void TryCall_with_ServiceResultTValue_test()
    {
        bool delegateRan = false;
        var test = new TestClass();

        int TestDelegate(bool arg)
        {
            delegateRan = arg;
            return 1;
        }

        var result = test.Test_TryCall<int>(CrudAction.Create, TestDelegate, true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.True);
        Assert.That(delegateRan, Is.True);
        Assert.That(result.Result, Is.EqualTo(1));
    }

    [Test]
    public void TryCall_with_invalid_ServiceResult_delegate_test()
    {
        bool delegateRan = false;
        var test = new TestClass();

        void TestDelegate(bool arg) => delegateRan = arg;

        Assert.Throws<InvalidOperationException>(() => _ = test.Test_TryCall<int>(CrudAction.Create, TestDelegate, true));
        Assert.That(delegateRan, Is.False);
    }

    [Test]
    public void TryCall_skips_on_prolog_result_test()
    {
        bool delegateRan = false;
        var test = new TestClass();

        ServiceResult? Stop(CrudAction crudAction, IEnumerable<Model>? entity) => FailureReason.Tamper;

        int TestDelegate(bool arg)
        {
            delegateRan = arg;
            return 1;
        }

        test.Configurator.AddProlog(Stop);
        var result = test.Test_TryCall<int>(CrudAction.Create, TestDelegate, true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(delegateRan, Is.False);
        Assert.That(result.Result, Is.EqualTo(default(int)));
        Assert.That(result.Reason, Is.EqualTo(FailureReason.Tamper));
    }
}
