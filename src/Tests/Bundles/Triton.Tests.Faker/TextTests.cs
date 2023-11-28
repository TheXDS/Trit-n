#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Faker;

namespace TheXDS.Triton.Tests.Faker;

public class TextTests
{
    [Test]
    public void Lorem_Test()
    {
        Assert.That(Text.Lorem(1), Is.Not.Empty);
        Assert.That(Text.Lorem(200).Length, Is.GreaterThan(Text.Lorem(100).Length));
    }

    [Test]
    public void Lorem_contract_Test()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Text.Lorem(0, 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Text.Lorem(1, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Text.Lorem(1, 1, 0));
    }
}
