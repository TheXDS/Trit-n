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
        Assert.IsInstanceOf<Person>(p);
        Assert.IsNotEmpty(p.FirstName);
        Assert.IsNotEmpty(p.Surname);
        Assert.IsInstanceOf<Gender>(p.Gender);
        Assert.IsInstanceOf<DateTime>(p.Birth);
        Assert.IsNotEmpty(p.UserName);
        Assert.IsNotEmpty(p.Name);
        Assert.IsNotEmpty(p.FullName);
        Assert.IsInstanceOf<double>(p.Age);
        Assert.IsTrue(p.Age.IsValid());
        Assert.IsTrue(p.Age.IsBetween(0, 110));

    }

    [Test]
    public void Adult_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.IsTrue(Person.Adult().Age.IsBetween(18, 60));
        }
    }
    
    [Test]
    public void Kid_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.IsTrue(Person.Kid().Age.IsBetween(5, 18));
        }
    }

    [Test]
    public void Baby_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            var b = Person.Baby();
            Assert.IsTrue(b.Age.IsBetween(0, 5));
        }
    }

    [Test]
    public void Old_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.IsTrue(Person.Old().Age.IsBetween(60, 110));
        }
    }
    
    [Test]
    public void FakeBirth_Test()
    {
        for (var j = 0; j < 1000; j++)
        {
            Assert.IsTrue(((DateTime.Today - Person.FakeBirth(20, 40)).TotalDays / 365.25).IsBetween(20, 40));
        }
    }
}
