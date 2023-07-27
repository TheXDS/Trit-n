#pragma warning disable CS1591

using NUnit.Framework;
using System.Collections;
using System.Linq.Expressions;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Services;

public class QueryServiceResultTests
{
    private static QueryServiceResult<User> GetEmpty() => new();

    private static QueryServiceResult<User> GetSingle() => new(new[] { new User("1", "One") }.AsQueryable());

    [Test]
    public void Empty_QueryResult_fails() 
    {
        Assert.That(GetEmpty().Success, Is.False);
    }

    [Test]
    public void ElementType_throws_if_null()
    {
        Assert.That(() => GetEmpty().ElementType, Throws.InvalidOperationException);
    }

    [Test]
    public void Expression_throws_if_null()
    {
        Assert.That(() => GetEmpty().Expression, Throws.InvalidOperationException);
    }

    [Test]
    public void Provider_throws_if_null()
    {
        Assert.That(() => GetEmpty().Provider, Throws.InvalidOperationException);
    }

    [Test]
    public void GetEnumerator_from_IEnumerable_T_throws_if_null()
    {
        Assert.That(() => ((IEnumerable)GetEmpty()).GetEnumerator(), Throws.InvalidOperationException);
    }

    [Test]
    public void GetEnumerator_from_IEnumerable_throws_if_null()
    {
        Assert.That(() => ((IEnumerable<User>)GetEmpty()).GetEnumerator(), Throws.InvalidOperationException);
    }

    [Test]
    public void GetEnumerator_from_IEnumerable_T_returns_enumerator()
    {
        Assert.That(((IEnumerable)GetSingle()).GetEnumerator(), Is.InstanceOf<IEnumerator>());
    }

    [Test]
    public void GetEnumerator_returns_enumerator()
    {
        Assert.That(GetSingle().GetEnumerator(), Is.InstanceOf<IEnumerator<User>>());
    }

    [Test]
    public void GetEnumerator_from_IEnumerable_returns_enumerator()
    {
        Assert.That(((IEnumerable<User>)GetSingle()).GetEnumerator(), Is.InstanceOf<IEnumerator<User>>());
    }

    [Test]
    public void ElementType_returns_valid_element_type()
    {
        Assert.That(GetSingle().ElementType, Is.EqualTo(typeof(User)));
    }

    [Test]
    public void ElementType_returns_valid_Expression()
    {
        Assert.That(GetSingle().Expression, Is.InstanceOf<Expression>());
    }

    [Test]
    public void ElementType_returns_valid_Provider()
    {
        Assert.That(GetSingle().Provider, Is.InstanceOf<IQueryProvider>());
    }

    [Test]
    public void QueryServiceResult_from_exception()
    {
        var ex = new Exception("Error XYZ");
        var result = new QueryServiceResult<User>(ex);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo(ex.Message));
        Assert.That(result.Reason, Is.Not.Null);
        Assert.That((int)result.Reason!.Value, Is.EqualTo(ex.HResult));
    }

    [Test]
    public void QueryServiceResult_from_FailureReason()
    {
        var r = new QueryServiceResult<User>(FailureReason.ConcurrencyFailure);
        Assert.That(r.Success, Is.False);
        Assert.AreEqual(FailureReason.ConcurrencyFailure, r.Reason);
    }

    [Test]
    public void QueryServiceResult_from_error_message()
    {
        string message = $"Test {Guid.NewGuid()}";
        var r = new QueryServiceResult<User>(message);
        Assert.That(r.Success, Is.False);
        Assert.AreEqual(message, r.Message);
    }

    [Test]
    public void Fail_with_reason_and_message()
    {
        var r = new QueryServiceResult<User>(FailureReason.ConcurrencyFailure, "Test");
        Assert.That(r.Success, Is.False);
        Assert.AreEqual("Test", r.Message);
        Assert.AreEqual(FailureReason.ConcurrencyFailure, r.Reason);
    }

    [Test]
    public void QueryServiceResult_from_exception_implicit_conversion()
    {
        var ex = new Exception("Error XYZ");
        var result = (QueryServiceResult<User>)ex;
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ex.Message, result.Message);
        Assert.IsNotNull(result.Reason);
        Assert.AreEqual(ex.HResult, (int)result.Reason!);
    }

    [Test]
    public void QueryServiceResult_from_string_implicit_conversion()
    {
        var msg = "Error X";
        var result = (QueryServiceResult<User>)msg;
        Assert.IsFalse(result.Success);
        Assert.AreEqual(msg, result.Message);
    }

    [Test]
    public void QueryServiceResult_to_bool_implicit_conversion()
    {
        var result = GetEmpty();
        Assert.IsFalse((bool)result);

        result = GetSingle();
        Assert.IsTrue((bool)result);
    }
}
