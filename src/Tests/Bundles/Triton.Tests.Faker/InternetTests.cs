#pragma warning disable CS1591

using NUnit.Framework;
using System.Text.RegularExpressions;
using TheXDS.Triton.Faker;

namespace TheXDS.Triton.Tests.Faker;

public class InternetTests
{
    [Test]
    public void FakeUsername_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            Assert.That(Internet.FakeUsername(), Is.Not.Empty);
        }
    }

    [Test]
    public void FakeEmail_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var e = Internet.FakeEmail();
            Assert.That(e, Is.Not.Empty);
            Assert.That(Regex.IsMatch(e, ".+@.+[.].{2,}"), Is.True);
        }
    }

    [Test]
    public void FakeEmail_with_person_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var e = Internet.FakeEmail(Person.Someone());
            Assert.That(e, Is.Not.Empty);
            Assert.That(Regex.IsMatch(e, ".+@.+[.].{2,}"), Is.True);
        }
    }
}
