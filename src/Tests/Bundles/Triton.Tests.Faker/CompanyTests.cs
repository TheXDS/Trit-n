#pragma warning disable CS1591

using NUnit.Framework;
using System.Text.RegularExpressions;
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

    [Test]
    public void RndEmployee_Test()
    {
        foreach (var _ in Enumerable.Range(0, 10))
        {
            Company c = new();
            foreach (var __ in Enumerable.Range(0, 100))
            {
                var e = c.RndEmployee();
                Assert.IsInstanceOf<Employee>(e);
                Assert.IsTrue(Regex.IsMatch(e.Email, ".+@.+[.].{2,}"));
                Assert.IsTrue(e.Email.EndsWith($"@{c.DomainName}"));
            }
        }
    }

    [Test]
    public void RndChief_Test()
    {
        foreach (var _ in Enumerable.Range(0, 10))
        {
            Company c = new();
            foreach (var __ in Enumerable.Range(0, 100))
            {
                var e = c.RndChief();
                Assert.IsInstanceOf<Employee>(e);
                Assert.IsTrue(Regex.IsMatch(e.Email, ".+@.+[.].{2,}"));
                Assert.IsTrue(e.Email.EndsWith($"@{c.DomainName}"));
            }
        }
    }
}
