#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Tests.Models;

public class TimestampModelTests
{
    private class TestClass : TimestampModel<int>
    {
        public TestClass()
        {
        }

        public TestClass(DateTime timestamp) : base(timestamp)
        {
        }
    }

    [Test]
    public void Ctor_Test()
    {
        Assert.That(new TestClass().Timestamp, Is.EqualTo(default(DateTime)));

        var n = DateTime.Now;
        Assert.That(new TestClass(n).Timestamp, Is.EqualTo(n));
    }
}