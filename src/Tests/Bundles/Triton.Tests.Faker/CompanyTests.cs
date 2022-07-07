#pragma warning disable CS1591

using NUnit.Framework;
using System;
using System.Linq;
using TheXDS.Triton.Faker;

namespace TheXDS.Triton.Tests.Faker;

public class CompanyTests
{
    [Test]
    public void Company_has_fake_data()
    {
        foreach (var _ in Enumerable.Range(0, 1000))
        {
            Company c = new();
            Assert.IsNotNull(c);
            Assert.IsNotEmpty(c.Name);
            Assert.IsNotNull(c.Address);
            Assert.IsNotEmpty(c.DomainName);
            Assert.IsNotEmpty(c.Website);
            Assert.IsTrue(Uri.IsWellFormedUriString(c.Website, UriKind.Absolute));
        }
    }
}
