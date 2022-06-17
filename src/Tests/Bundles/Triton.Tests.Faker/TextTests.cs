#pragma warning disable CS1591

using NUnit.Framework;
using System;
using TheXDS.Triton.Faker;

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
}
