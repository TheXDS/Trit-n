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
            Assert.IsNotEmpty(Internet.FakeUsername());
        }
    }

    [Test]
    public void FakeEmail_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var e = Internet.FakeEmail();
            Assert.IsNotEmpty(e);
            Assert.IsTrue(Regex.IsMatch(e, ".+@.+[.].{2,}"));
        }
    }

    [Test]
    public void FakeEmail_with_person_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var e = Internet.FakeEmail(Person.Someone());
            Assert.IsNotEmpty(e);
            Assert.IsTrue(Regex.IsMatch(e, ".+@.+[.].{2,}"));
        }
    }
}
