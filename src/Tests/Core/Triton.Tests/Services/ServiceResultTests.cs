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
        Assert.That(result.Success, Is.False);
        Assert.That(ex.Message, Is.EqualTo(result.Message));
        Assert.That(result.Reason, Is.Not.Null);
        Assert.That(ex.HResult, Is.EqualTo((int)result.Reason!));
    }

    [Test]
    public void ServiceResult_from_exception_implicit_conversion_test()
    {
        var ex = new Exception("Error XYZ");
        var result = (ServiceResult)ex;
        Assert.That(result.Success, Is.False);
        Assert.That(ex.Message, Is.EqualTo(result.Message));
        Assert.That(result.Reason, Is.Not.Null);
        Assert.That(ex.HResult, Is.EqualTo((int)result.Reason!));
    }

    [Test]
    public void ServiceResult_from_string_implicit_conversion_test()
    {
        var msg = "Error X";
        var result = (ServiceResult)msg;
        Assert.That(result.Success, Is.False);
        Assert.That(msg, Is.EqualTo(result.Message));
    }

    [Test]
    public void ServiceResult_from_bool_implicit_conversion_test()
    {
        var result = (ServiceResult)false;
        Assert.That((bool)result, Is.False);

        result = (ServiceResult)true;
        Assert.That((bool)result);
    }

    [Test]
    public void ServiceResult_to_bool_implicit_conversion_test()
    {
        var result = (ServiceResult)false;
        Assert.That(result.Success, Is.False);

        result = (ServiceResult)true;
        Assert.That(result.Success);
    }

    [Test]
    public void ServiceResult_to_string_implicit_conversion_test()
    {
        var msg = "Error X";
        var result = (ServiceResult)msg;
        Assert.That(msg, Is.EqualTo((string)result));
    }

    [Test]
    public void ToString_test()
    {
        var msg = "Error X";
        var result = (ServiceResult)msg;
        Assert.That(msg, Is.EqualTo(result.ToString()));
    }

    [Test]
    public void Equals_with_ServiceResult_test()
    {
        Assert.That(ServiceResult.Ok.Equals((ServiceResult)FailureReason.Unknown), Is.False);
        Assert.That(((ServiceResult)FailureReason.ServiceFailure).Equals((ServiceResult)FailureReason.ServiceFailure));
    }

    [Test]
    public void Equals_with_Exception_test()
    {
        Assert.That(((ServiceResult)new InvalidOperationException()).Equals(new StackOverflowException()), Is.False);
        Assert.That(((ServiceResult)new NullReferenceException()).Equals(new NullReferenceException()));
    }

    [Test]
    public void Equals_with_object_test()
    {
        Assert.That(ServiceResult.Ok.Equals((object)false), Is.False);
        Assert.That(ServiceResult.Ok.Equals((object)true));

        Assert.That(ServiceResult.Ok.Equals((object)(ServiceResult)FailureReason.Unknown), Is.False);
        Assert.That(((ServiceResult)FailureReason.ServiceFailure).Equals((object)(ServiceResult)FailureReason.ServiceFailure));

        Assert.That(ServiceResult.Ok.Equals((Exception?)null));
        Assert.That(((ServiceResult)new InvalidOperationException()).Equals((object)new StackOverflowException()), Is.False);
        Assert.That(((ServiceResult)new NullReferenceException()).Equals((object)new NullReferenceException()));

        Assert.That(ServiceResult.Ok!.Equals((object?)null), Is.False);
        Assert.That(ServiceResult.Ok!.Equals(new object()), Is.False);
    }

    [Test]
    public void Success_result_with_message_test()
    {
        var r = new ServiceResult("test");
        Assert.That(r.Success);
        Assert.That("test", Is.EqualTo(r.Message));
    }

    [Test]
    public void Custom_reason_message_test()
    {
        var r = ServiceResult.FailWith<ServiceResult>((FailureReason)0x08070605);
        Assert.That(r.Success, Is.False);
        Assert.That("0x08070605", Is.EqualTo(r.Message));
        Assert.That(0x08070605, Is.EqualTo((int)r.Reason!));
    }

    [Test]
    public void Fail_from_Exception_test()
    {
        var ex = new IOException("Test");
        var r = ServiceResult.FailWith<ServiceResult>(ex);
        Assert.That(r.Success, Is.False);
        Assert.That("Test", Is.EqualTo(r.Message));
        Assert.That(ex.HResult, Is.EqualTo((int)r.Reason!));
    }

    [Test]
    public void Fail_with_message_test()
    {
        var r = ServiceResult.FailWith<ServiceResult>("Test");
        Assert.That(r.Success, Is.False);
        Assert.That("Test", Is.EqualTo(r.Message));
        Assert.That(FailureReason.Unknown, Is.EqualTo(r.Reason));
    }

    [Test]
    public void Fail_with_reason_and_message_test()
    {
        var r = new ServiceResult(FailureReason.ConcurrencyFailure, "Test");
        Assert.That(r.Success, Is.False);
        Assert.That("Test", Is.EqualTo(r.Message));
        Assert.That(FailureReason.ConcurrencyFailure, Is.EqualTo(r.Reason));
    }
}
