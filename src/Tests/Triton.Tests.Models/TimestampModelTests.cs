#pragma warning disable CS1591
namespace TheXDS.Triton.Tests.Models;
using TheXDS.Triton.Models.Base;
using System;
using NUnit.Framework;

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
        Assert.AreEqual(default(DateTime), new TestClass().Timestamp);

        var n = DateTime.Now;
        Assert.AreEqual(n, new TestClass(n).Timestamp);
    }
}