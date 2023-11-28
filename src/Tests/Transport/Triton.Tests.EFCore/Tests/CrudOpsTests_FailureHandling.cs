#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.EFCore.Tests;

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
        Assert.That(post.Success, Is.False);
        Assert.That(post.ReturnValue, Is.Null);
        Assert.That(post.Reason.HasValue, Is.True);
        return post.Reason!.Value;
    }
}