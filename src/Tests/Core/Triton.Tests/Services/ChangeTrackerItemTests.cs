#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Exceptions;
using TheXDS.Triton.Services;
using TheXDS.Triton.Tests.Models;

namespace TheXDS.Triton.Tests.Services;

public class ChangeTrackerItemTests
{
    [TestCase(true, true, ChangeTrackerChangeType.NoChange)]
    [TestCase(true, false, ChangeTrackerChangeType.Create)]
    [TestCase(false, false, ChangeTrackerChangeType.Update)]
    [TestCase(false, true, ChangeTrackerChangeType.Delete)]
    public void ChangeType_returns_expected_value(bool oldIsNull, bool newIsNull, ChangeTrackerChangeType expectedResult)
    {
        var item = new ChangeTrackerItem(oldIsNull ? null : new User() { Id = "A" }, newIsNull ? null : new User() { Id = "B" });
        Assert.That(item.ChangeType, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Ctor_throws_on_model_mismatch()
    {
        var user = new User();
        var post = new Post();
        Assert.That(() => _ = new ChangeTrackerItem(user, post), Throws.InstanceOf<ModelTypeMismatchException>());
    }
}
