#pragma warning disable CS1591

using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
using TheXDS.MCART.Helpers;
using TheXDS.Triton.Fakers;

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
    }

    [Test]
    public void Adult_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            Assert.IsTrue(Person.Adult().Age.IsBetween(18, 60));
        }
    }
    
    [Test]
    public void Kid_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            Assert.IsTrue(Person.Kid().Age.IsBetween(5, 18));
        }
    }

    [Test]
    public void Baby_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            Assert.IsTrue(Person.Baby().Age.IsBetween(0, 5));
        }
    }

    [Test]
    public void Old_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            Assert.IsTrue(Person.Old().Age.IsBetween(60, 110));
        }
    }
    
    [Test]
    public void FakeBirth_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            Assert.IsTrue(((DateTime.Today - Person.FakeBirth(20, 40)).TotalDays / 365.25).IsBetween(20, 40));
        }
    }

    [Test]
    public void FakeUsername_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            Assert.IsNotEmpty(Person.FakeUsername());
        }
    }
    
    [Test]
    public void FakeEmail_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var e = Person.FakeEmail();
            Assert.IsNotEmpty(e);
            Assert.IsTrue(Regex.IsMatch(e, ".+@.+[.].{2,}"));
        }
    }
    
    [Test]
    public void FakeEmail_with_person_Test()
    {
        for (var j = 0; j < 100; j++)
        {
            var e = Person.FakeEmail(Person.Someone());
            Assert.IsNotEmpty(e);
            Assert.IsTrue(Regex.IsMatch(e, ".+@.+[.].{2,}"));
        }
    }
}