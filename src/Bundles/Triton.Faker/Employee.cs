using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Represents an employee generated randomly.
/// </summary>
public class Employee : Person
{
    /// <summary>
    /// Gets the email address of the employee.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets the position of the employee within the company.
    /// </summary>
    public string Position { get; }

    /// <summary>
    /// Generates a chief employee randomly.
    /// </summary>
    /// <param name="company">
    /// The company for which the employee is generated to work.
    /// </param>
    /// <returns>
    /// A new instance of the class <see cref="Employee"/>.
    /// </returns>
    public static Employee GetChief(Company company)
    {
        return ChiefFromPerson(Someone(35, 80), company);
    }

    /// <summary>
    /// Generates an employee randomly.
    /// </summary>
    /// <param name="company">
    /// The company for which the employee is generated to work.
    /// </param>
    /// <returns>
    /// A new instance of the class <see cref="Employee"/>.
    /// </returns>
    public static Employee Get(Company company)
    {
        return FromPerson(Adult(), company);
    }

    /// <summary>
    /// Converts an instance of the class <see cref="Person"/> to an instance
    /// of <see cref="Employee"/>, specifying the company for which the
    /// employee is generated to work.
    /// </summary>
    /// <param name="person">The person to convert to an employee.</param>
    /// <param name="company">
    /// The company for which the employee is generated to work.
    /// </param>
    /// <returns>
    /// A new instance of the class <see cref="Employee"/>.
    /// </returns>
    public static Employee FromPerson(Person person, Company company)
    {
        return new(_rnd.CoinFlip() ? StringTables.WorkPositions.Pick() : "Collaborator", person, company);
    }

    /// <summary>
    /// Converts an instance of the class <see cref="Person"/> to an instance
    /// of <see cref="Employee"/> with chief rank, specifying the company for
    /// which the employee is generated to work.
    /// </summary>
    /// <param name="person">The person to convert to an employee.</param>
    /// <param name="company">
    /// The company for which the employee is generated to work.
    /// </param>
    /// <returns>
    /// A new instance of the class <see cref="Employee"/>.
    /// </returns>
    public static Employee ChiefFromPerson(Person person, Company company)
    {
        return new(StringTables.ChiefPositions.Pick(), person, company);
    }

    private Employee(string position, Person person, Company company)
        : base(person.FirstName, person.Surname, person.Gender, person.Birth)
    {
        Email = $"{person.UserName}@{company.DomainName}";
        Position = position;
        SetUserName(person.UserName);
    }
}
