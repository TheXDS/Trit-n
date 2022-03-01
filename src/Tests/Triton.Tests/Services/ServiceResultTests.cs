#pragma warning disable CS1591

using NUnit.Framework;
using System.IO;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.InMemory.Services
{
    public class ServiceResultTests
    {
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
}
