#pragma warning disable CS1591

using NUnit.Framework;
using System.Text.RegularExpressions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Math;
using TheXDS.Triton.Faker;

namespace TheXDS.Triton.Tests.Faker;

public class EmployeeTests
{
    [Test]
    public void Get_Test()
    {
        Company company = new();
        var employee = Employee.Get(company);

        Assert.That(employee, Is.InstanceOf<Employee>());
        Assert.That(employee.FirstName, Is.Not.Empty);
        Assert.That(employee.Surname, Is.Not.Empty);
        Assert.That(employee.Gender, Is.InstanceOf<Gender>());
        Assert.That(employee.Birth, Is.InstanceOf<DateTime>());
        Assert.That(employee.UserName, Is.Not.Empty);
        Assert.That(employee.Name, Is.Not.Empty);
        Assert.That(employee.FullName, Is.Not.Empty);
        Assert.That(employee.Age, Is.InstanceOf<double>());
        Assert.That(employee.Age.IsValid(), Is.True);
        Assert.That(employee.Age.IsBetween(18, 80), Is.True);
        Assert.That(employee.Email, Is.Not.Empty);
        Assert.That(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"), Is.True);
        Assert.That(employee.Email.EndsWith($"@{company.DomainName}"), Is.True);
        Assert.That(employee.Position, Is.Not.Empty);
    }

    [Test]
    public void GetChief_Test()
    {
        Company company = new();
        var employee = Employee.GetChief(company);

        Assert.That(employee, Is.InstanceOf<Employee>());
        Assert.That(employee.FirstName, Is.Not.Empty);
        Assert.That(employee.Surname, Is.Not.Empty);
        Assert.That(employee.Gender, Is.InstanceOf<Gender>());
        Assert.That(employee.Birth, Is.InstanceOf<DateTime>());
        Assert.That(employee.UserName, Is.Not.Empty);
        Assert.That(employee.Name, Is.Not.Empty);
        Assert.That(employee.FullName, Is.Not.Empty);
        Assert.That(employee.Age, Is.InstanceOf<double>());
        Assert.That(employee.Age.IsValid(), Is.True);
        Assert.That(employee.Age.IsBetween(18, 80), Is.True);
        Assert.That(employee.Email, Is.Not.Empty);
        Assert.That(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"), Is.True);
        Assert.That(employee.Email.EndsWith($"@{company.DomainName}"), Is.True);
        Assert.That(employee.Position, Is.Not.Empty);
    }

    [Test]
    public void FromPerson_Test()
    {
        Person person = Person.Adult();
        Company company = new();
        var employee = Employee.FromPerson(person, company);

        Assert.That(employee, Is.InstanceOf<Employee>());
        Assert.That(employee.FirstName, Is.EqualTo(person.FirstName));
        Assert.That(employee.Surname, Is.EqualTo(person.Surname));
        Assert.That(employee.Gender, Is.EqualTo(person.Gender));
        Assert.That(employee.Birth, Is.EqualTo(person.Birth));
        Assert.That(employee.UserName, Is.EqualTo(person.UserName));
        Assert.That(employee.Name, Is.EqualTo(person.Name));
        Assert.That(employee.FullName, Is.EqualTo(person.FullName));
        Assert.That(employee.Age, Is.EqualTo(person.Age));
        Assert.That(employee.Email, Is.Not.Empty);
        Assert.That(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"), Is.True);
        Assert.That(employee.Email.EndsWith($"@{company.DomainName}"), Is.True);
        Assert.That(employee.Email.StartsWith(person.UserName), Is.True);
        Assert.That(employee.Position, Is.Not.Empty);
    }

    [Test]
    public void ChiefFromPerson_Test()
    {
        Person person = Person.Adult();
        Company company = new();
        var employee = Employee.ChiefFromPerson(person, company);

        Assert.That(employee, Is.InstanceOf<Employee>());
        Assert.That(employee.FirstName, Is.EqualTo(person.FirstName));
        Assert.That(employee.Surname, Is.EqualTo(person.Surname));
        Assert.That(employee.Gender, Is.EqualTo(person.Gender));
        Assert.That(employee.Birth, Is.EqualTo(person.Birth));
        Assert.That(employee.UserName, Is.EqualTo(person.UserName));
        Assert.That(employee.Name, Is.EqualTo(person.Name));
        Assert.That(employee.FullName, Is.EqualTo(person.FullName));
        Assert.That(employee.Age, Is.EqualTo(person.Age));
        Assert.That(employee.Email, Is.Not.Empty);
        Assert.That(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"), Is.True);
        Assert.That(employee.Email.EndsWith($"@{company.DomainName}"), Is.True);
        Assert.That(employee.Email.StartsWith(person.UserName), Is.True);
        Assert.That(employee.Position, Is.Not.Empty);
    }
}