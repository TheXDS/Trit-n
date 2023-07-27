#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.Services;

public class ServiceResultTests
{
    [Test]
    public void ServiceResult_from_exception_test()
    {
        var ex = new Exception("Error XYZ");
        var result = new ServiceResult(ex);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ex.Message, result.Message);
        Assert.IsNotNull(result.Reason);
        Assert.AreEqual(ex.HResult, (int)result.Reason!);
    }

    [Test]
    public void ServiceResult_from_exception_implicit_conversion_test()
    {
        var ex = new Exception("Error XYZ");
        var result = (ServiceResult)ex;
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ex.Message, result.Message);
        Assert.IsNotNull(result.Reason);
        Assert.AreEqual(ex.HResult, (int)result.Reason!);
    }

    [Test]
    public void ServiceResult_from_string_implicit_conversion_test()
    {
        var msg = "Error X";
        var result = (ServiceResult)msg;
        Assert.IsFalse(result.Success);
        Assert.AreEqual(msg, result.Message);
    }

    [Test]
    public void ServiceResult_from_bool_implicit_conversion_test()
    {
        var result = (ServiceResult)false;
        Assert.IsFalse((bool)result);

        result = (ServiceResult)true;
        Assert.IsTrue((bool)result);
    }

    [Test]
    public void ServiceResult_to_bool_implicit_conversion_test()
    {
        var result = (ServiceResult)false;
        Assert.IsFalse(result.Success);

        result = (ServiceResult)true;
        Assert.IsTrue(result.Success);
    }

    [Test]
    public void ServiceResult_to_string_implicit_conversion_test()
    {
        var msg = "Error X";
        var result = (ServiceResult)msg;
        Assert.AreEqual(msg, (string)result);
    }

    [Test]
    public void ToString_test()
    {
        var msg = "Error X";
        var result = (ServiceResult)msg;
        Assert.AreEqual(msg, result.ToString());
    }

    [Test]
    public void Equals_with_ServiceResult_test()
    {
        Assert.IsFalse(ServiceResult.Ok.Equals((ServiceResult)FailureReason.Unknown));
        Assert.IsTrue(((ServiceResult)FailureReason.ServiceFailure).Equals((ServiceResult)FailureReason.ServiceFailure));
    }

    [Test]
    public void Equals_with_Exception_test()
    {
        Assert.IsFalse(((ServiceResult)new InvalidOperationException()).Equals(new StackOverflowException()));
        Assert.IsTrue(((ServiceResult)new NullReferenceException()).Equals(new NullReferenceException()));
    }

    [Test]
    public void Equals_with_object_test()
    {
        Assert.IsFalse(ServiceResult.Ok.Equals((object)false));
        Assert.IsTrue(ServiceResult.Ok.Equals((object)true));

        Assert.IsFalse(ServiceResult.Ok.Equals((object)(ServiceResult)FailureReason.Unknown));
        Assert.IsTrue(((ServiceResult)FailureReason.ServiceFailure).Equals((object)(ServiceResult)FailureReason.ServiceFailure));

        Assert.IsTrue(ServiceResult.Ok.Equals((Exception?)null));
        Assert.IsFalse(((ServiceResult)new InvalidOperationException()).Equals((object)new StackOverflowException()));
        Assert.IsTrue(((ServiceResult)new NullReferenceException()).Equals((object)new NullReferenceException()));

        Assert.IsFalse(ServiceResult.Ok!.Equals((object?)null));
        Assert.IsFalse(ServiceResult.Ok!.Equals(new object()));
    }

    [Test]
    public void Success_result_with_message_test()
    {
        var r = new ServiceResult("test");
        Assert.IsTrue(r.Success);
        Assert.AreEqual("test", r.Message);
    }

    [Test]
    public void Custom_reason_message_test()
    {
        var r = ServiceResult.FailWith<ServiceResult>((FailureReason)0x08070605);
        Assert.False(r.Success);
        Assert.AreEqual("0x08070605", r.Message);
        Assert.AreEqual(0x08070605, (int)r.Reason!);
    }

    [Test]
    public void Fail_from_Exception_test()
    {
        var ex = new IOException("Test");
        var r = ServiceResult.FailWith<ServiceResult>(ex);
        Assert.False(r.Success);
        Assert.AreEqual("Test", r.Message);
        Assert.AreEqual(ex.HResult, (int)r.Reason!);
    }

    [Test]
    public void Fail_with_message_test()
    {
        var r = ServiceResult.FailWith<ServiceResult>("Test");
        Assert.False(r.Success);
        Assert.AreEqual("Test", r.Message);
        Assert.AreEqual(FailureReason.Unknown, r.Reason);
    }

    [Test]
    public void Fail_with_reason_and_message_test()
    {
        var r = new ServiceResult(FailureReason.ConcurrencyFailure, "Test");
        Assert.False(r.Success);
        Assert.AreEqual("Test", r.Message);
        Assert.AreEqual(FailureReason.ConcurrencyFailure, r.Reason);
    }
}
