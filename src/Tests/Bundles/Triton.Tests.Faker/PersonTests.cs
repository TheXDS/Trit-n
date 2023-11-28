#pragma warning disable CS1591

using NUnit.Framework;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Math;
using TheXDS.Triton.Faker;

namespace TheXDS.Triton.Tests.Faker;

public class PersonTests
{
    [Test]
    public void Someone_Test()
    {
        var p = Person.Someone();
        Assert.That(p, Is.InstanceOf<Person>());
        Assert.That(p.FirstName, Is.Not.Empty);
        Assert.That(p.Surname, Is.Not.Empty);
        Assert.That(p.Gender, Is.InstanceOf<Gender>());
        Assert.That(p.Birth, Is.InstanceOf<DateTime>());
        Assert.That(p.UserName, Is.Not.Empty);
        Assert.That(p.Name, Is.Not.Empty);
        Assert.That(p.FullName, Is.Not.Empty);
        Assert.That(p.Age, Is.InstanceOf<double>());
        Assert.That(p.Age.IsValid(), Is.True);
        Assert.That(p.Age.IsBetween(0, 110), Is.True);
    }

    [Test]
    public void Adult_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.That(Person.Adult().Age.IsBetween(18, 60), Is.True);
        }
    }

    [Test]
    public void Kid_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.That(Person.Kid().Age.IsBetween(5, 18), Is.True);
        }
    }

    [Test]
    public void Baby_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            var b = Person.Baby();
            Assert.That(b.Age.IsBetween(0, 5), Is.True);
        }
    }

    [Test]
    public void Old_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.That(Person.Old().Age.IsBetween(60, 110), Is.True);
        }
    }

    [Test]
    public void FakeBirth_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.That(((DateTime.Today - Person.FakeBirth(20, 40)).TotalDays / 365.25).IsBetween(20, 40), Is.True);
        }
    }
}
