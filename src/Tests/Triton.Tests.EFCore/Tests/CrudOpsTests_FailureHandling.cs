#pragma warning disable CS1591

using NUnit.Framework;
using System.Collections.Generic;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests
{
    public partial class CrudOpsTests
    {
        static IEnumerable<TestCaseData> SimpleReadFailures()
        {
            yield return new TestCaseData(999L).Returns(FailureReason.NotFound);
            yield return new TestCaseData(1).Returns(FailureReason.BadQuery);
            yield return new TestCaseData("Abc").Returns(FailureReason.BadQuery);
        }

        [TestCaseSource(nameof(SimpleReadFailures))]
        public FailureReason FailToRead_Test(object id)
        {
            using var t = _srv.GetTransaction();
            var post = t.Read<Post>(id);
            Assert.IsFalse(post.Success);
            Assert.IsNull(post.ReturnValue);
            Assert.True(post.Reason.HasValue);
            return post.Reason!.Value;
        }
    }
}