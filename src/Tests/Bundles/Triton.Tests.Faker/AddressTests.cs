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
