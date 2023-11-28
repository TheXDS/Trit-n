#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Faker;

namespace TheXDS.Triton.Tests.Faker;

public class AddressTests
{
    [Test]
    public void GetAddress_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var a = Address.NewAddress();
            Assert.That(a.AddressLine, Is.Not.Empty);
            if (a.AddressLine2 is not null) Assert.That(a.AddressLine2, Is.Not.Empty);
            Assert.That(a.City, Is.Not.Empty);
            Assert.That(a.Country, Is.Not.Empty);
            Assert.That(a.Zip, Is.AssignableFrom<ushort>());

            var s = a.ToString();
            Assert.That(s.Contains(a.AddressLine), Is.True);
            if (a.AddressLine2 is not null) Assert.That(s.Contains(a.AddressLine), Is.True);
            Assert.That(s.Contains(a.City), Is.True);
            Assert.That(s.Contains(a.Country), Is.True);
            Assert.That(s.Contains(a.Zip.ToString()), Is.True);
        }
    }
}
