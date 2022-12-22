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

        Assert.IsInstanceOf<Employee>(employee);
        Assert.IsNotEmpty(employee.FirstName);
        Assert.IsNotEmpty(employee.Surname);
        Assert.IsInstanceOf<Gender>(employee.Gender);
        Assert.IsInstanceOf<DateTime>(employee.Birth);
        Assert.IsNotEmpty(employee.UserName);
        Assert.IsNotEmpty(employee.Name);
        Assert.IsNotEmpty(employee.FullName);
        Assert.IsInstanceOf<double>(employee.Age);
        Assert.IsTrue(employee.Age.IsValid());
        Assert.IsTrue(employee.Age.IsBetween(18, 80));
        Assert.IsNotEmpty(employee.Email);
        Assert.IsTrue(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"));
        Assert.IsTrue(employee.Email.EndsWith($"@{company.DomainName}"));
        Assert.IsNotEmpty(employee.Position);
    }

    [Test]
    public void GetChief_Test()
    {
        Company company = new();
        var employee = Employee.GetChief(company);

        Assert.IsInstanceOf<Employee>(employee);
        Assert.IsNotEmpty(employee.FirstName);
        Assert.IsNotEmpty(employee.Surname);
        Assert.IsInstanceOf<Gender>(employee.Gender);
        Assert.IsInstanceOf<DateTime>(employee.Birth);
        Assert.IsNotEmpty(employee.UserName);
        Assert.IsNotEmpty(employee.Name);
        Assert.IsNotEmpty(employee.FullName);
        Assert.IsInstanceOf<double>(employee.Age);
        Assert.IsTrue(employee.Age.IsValid());
        Assert.IsTrue(employee.Age.IsBetween(18, 80));
        Assert.IsNotEmpty(employee.Email);
        Assert.IsTrue(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"));
        Assert.IsTrue(employee.Email.EndsWith($"@{company.DomainName}"));
        Assert.IsNotEmpty(employee.Position);
    }

    [Test]
    public void FromPerson_Test()
    {
        Person person = Person.Adult();
        Company company = new();
        var employee = Employee.FromPerson(person, company);

        Assert.IsInstanceOf<Employee>(employee);
        Assert.AreEqual(employee.FirstName, person.FirstName);
        Assert.AreEqual(employee.Surname, person.Surname);
        Assert.AreEqual(employee.Gender, person.Gender);
        Assert.AreEqual(employee.Birth, person.Birth);
        Assert.AreEqual(employee.UserName, person.UserName);
        Assert.AreEqual(employee.Name, person.Name);
        Assert.AreEqual(employee.FullName, person.FullName);
        Assert.AreEqual(employee.Age, person.Age);
        Assert.IsNotEmpty(employee.Email);
        Assert.IsTrue(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"));
        Assert.IsTrue(employee.Email.EndsWith($"@{company.DomainName}"));
        Assert.IsTrue(employee.Email.StartsWith(person.UserName));
        Assert.IsNotEmpty(employee.Position);
    }

    [Test]
    public void ChiefFromPerson_Test()
    {
        Person person = Person.Adult();
        Company company = new();
        var employee = Employee.ChiefFromPerson(person, company);

        Assert.IsInstanceOf<Employee>(employee);
        Assert.AreEqual(employee.FirstName, person.FirstName);
        Assert.AreEqual(employee.Surname, person.Surname);
        Assert.AreEqual(employee.Gender, person.Gender);
        Assert.AreEqual(employee.Birth, person.Birth);
        Assert.AreEqual(employee.UserName, person.UserName);
        Assert.AreEqual(employee.Name, person.Name);
        Assert.AreEqual(employee.FullName, person.FullName);
        Assert.AreEqual(employee.Age, person.Age);
        Assert.IsNotEmpty(employee.Email);
        Assert.IsTrue(Regex.IsMatch(employee.Email, ".+@.+[.].{2,}"));
        Assert.IsTrue(employee.Email.EndsWith($"@{company.DomainName}"));
        Assert.IsTrue(employee.Email.StartsWith(person.UserName));
        Assert.IsNotEmpty(employee.Position);
    }
}