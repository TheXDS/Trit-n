#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Services;

public class ServiceResult_T_Tests
{
    [Test]
    public void ServiceResult_with_string_Ctor()
    {
        var r = new ServiceResult<User>("Test");
        Assert.AreEqual("Test", r.Message);
    }
    [Test]
    public void ServiceResult_with_string_and_T_Ctor()
    {
        var u = new User("0", "Zero");
        var r = new ServiceResult<User>(u, "Test");
        Assert.AreEqual("Test", r.Message);
        Assert.That(r.ReturnValue, Is.SameAs(u));
    }

    [Test]
    public void ServiceResult_from_exception()
    {
        var ex = new Exception("Error XYZ");
        var result = new ServiceResult<User>(ex);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo(ex.Message));
        Assert.That(result.Reason, Is.Not.Null);
        Assert.That((int)result.Reason!.Value, Is.EqualTo(ex.HResult));
    }

    [Test]
    public void ServiceResult_from_FailureReason()
    {
        var r = new ServiceResult<User>(FailureReason.ConcurrencyFailure);
        Assert.That(r.Success, Is.False);
        Assert.AreEqual(FailureReason.ConcurrencyFailure, r.Reason);
    }

    [Test]
    public void ServiceResult_from_exception_implicit_conversion()
    {
        var ex = new Exception("Error XYZ");
        var result = (ServiceResult<User>)ex;
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ex.Message, result.Message);
        Assert.IsNotNull(result.Reason);
        Assert.AreEqual(ex.HResult, (int)result.Reason!);
    }

    [Test]
    public void QueryServiceResult_from_exception_implicit_conversion()
    {
        var result = (QueryServiceResult<User>)"Message";
        Assert.AreEqual("Message", result.Message);
        Assert.IsNotNull(result.Reason);
    }
}
