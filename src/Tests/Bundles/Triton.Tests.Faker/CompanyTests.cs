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
            Assert.That(c, Is.Not.Null);
            Assert.That(c.Name, Is.Not.Empty);
            Assert.That(c.Address, Is.Not.Null);
            Assert.That(c.DomainName, Is.Not.Empty);
            Assert.That(c.Website, Is.Not.Empty);
            Assert.That(Uri.IsWellFormedUriString(c.Website, UriKind.Absolute), Is.True);
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
                Assert.That(e, Is.InstanceOf<Employee>());
                Assert.That(Regex.IsMatch(e.Email, ".+@.+[.].{2,}"), Is.True);
                Assert.That(e.Email.EndsWith($"@{c.DomainName}"), Is.True);
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
                Assert.That(e, Is.InstanceOf<Employee>());
                Assert.That(Regex.IsMatch(e.Email, ".+@.+[.].{2,}"), Is.True);
                Assert.That(e.Email.EndsWith($"@{c.DomainName}"), Is.True);
            }
        }
    }
}
