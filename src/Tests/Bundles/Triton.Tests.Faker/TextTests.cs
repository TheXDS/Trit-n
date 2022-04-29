#pragma warning disable CS1591

using NUnit.Framework;
using System;
using TheXDS.Triton.Fakers;

namespace TheXDS.Triton.Tests.Faker;

public class TextTests
{
    [Test]
    public void Lorem_Test()
    {
        Assert.IsNotEmpty(Text.Lorem(1));
        Assert.Greater(Text.Lorem(200).Length, Text.Lorem(100).Length);
    }

    [Test]
    public void Lorem_contract_Test()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Text.Lorem(0, 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Text.Lorem(1, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Text.Lorem(1, 1, 0));
    }

    [Test]
    public void GetAddress_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var a = Text.GetAddress();
            Assert.IsNotEmpty(a.AddressLine);
            if (a.AddressLine2 is not null) Assert.IsNotEmpty(a.AddressLine2);
            Assert.IsNotEmpty(a.City);
            Assert.IsNotEmpty(a.Country);
            Assert.IsAssignableFrom<ushort>(a.Zip);

            var s = a.ToString();
            Assert.IsTrue(s.Contains(a.AddressLine));
            if (a.AddressLine2 is not null) Assert.IsTrue(s.Contains(a.AddressLine));
            Assert.IsTrue(s.Contains(a.City));
            Assert.IsTrue(s.Contains(a.Country));
            Assert.IsTrue(s.Contains(a.Zip.ToString()));
        }
    }
}
