using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Represents a randomly generated person, and includes static methods 
/// that generate new instances of this class with different kinds of 
/// randomized data.
/// </summary>
public class Person
{
    private string? _userName;

    /// <summary>
    /// Gets the first name of the person.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Gets the surname of the person.
    /// </summary>
    public string Surname { get; }

    /// <summary>
    /// Gets the biological gender of the person.
    /// </summary>
    public Gender Gender { get; }

    /// <summary>
    /// Gets the date of birth of the person.
    /// </summary>
    public DateTime Birth { get; }

    /// <summary>
    /// Gets a username generated for the person.
    /// </summary>
    public string UserName => _userName ??= Internet.FakeUsername(this);

    /// <summary>
    /// Gets the full name of the person, ex. "FirstName LastName".
    /// </summary>
    public string Name => string.Join(" ", FirstName, Surname);

    /// <summary>
    /// Gets the full name of the person in the format "LastName, FirstName".
    /// </summary>
    public string FullName => string.Join(", ", Surname, FirstName);

    /// <summary>
    /// Calculates and gets the age of the person as of today, in years.
    /// </summary>
    public double Age => (DateTime.Today - Birth).TotalDays / 365.25;

    /// <summary>
    /// Initializes a new instance of the class <see cref="Person"/>.
    /// </summary>
    /// <param name="firstName">First name of the person.</param>
    /// <param name="surname">Surname of the person.</param>
    /// <param name="gender">Biological gender of the person.</param>
    /// <param name="birth">Date of birth of the person.</param>
    protected Person(string firstName, string surname, Gender gender, DateTime birth)
    {
        FirstName = Capitalize(firstName);
        Surname = Capitalize(surname);
        Gender = gender;
        Birth = birth;
    }

    /// <summary>
    /// Sets the username for this instance of the class <see cref="Person"/>.
    /// </summary>
    /// <param name="userName">Username to set.</param>
    protected void SetUserName(string userName)
    {
        _userName = userName;
    }

    /// <summary>
    /// Generates a completely random baby.
    /// </summary>
    /// <returns>A completely random baby.</returns>
    public static Person Baby()
    {
        return Someone(0, 2);
    }

    /// <summary>
    /// Generates a completely random toddler.
    /// </summary>
    /// <returns>A completely random toddler.</returns>
    public static Person Toddler()
    {
        return Someone(2, 5);
    }

    /// <summary>
    /// Generates a completely random child.
    /// </summary>
    /// <returns>A completely random child.</returns>
    public static Person Child()
    {
        return Someone(5, 12);
    }

    /// <summary>
    /// Generates a completely random juvenile.
    /// </summary>
    /// <returns>A completely random juvenile.</returns>
    public static Person Juvenile()
    {
        return Someone(12, 18);
    }

    /// <summary>
    /// Generates a completely random adult.
    /// </summary>
    /// <returns>A completely random adult.</returns>
    public static Person Adult()
    {
        return Someone(18, 60);
    }

    /// <summary>
    /// Generates a completely random elderly person.
    /// </summary>
    /// <returns>A completely random elderly person.</returns>
    public static Person Old()
    {
        return Someone(60, 110);
    }

    /// <summary>
    /// Generates a completely random person.
    /// </summary>
    /// <param name="minAge">Minimum age of the person.</param>
    /// <param name="maxAge">Maximum age of the person.</param>
    /// <returns>
    /// A completely random person whose age falls within the specified range.
    /// </returns>
    public static Person Someone(int minAge, int maxAge)
    {
        var m = _rnd.CoinFlip();
        return new Person(
            (m ? StringTables.MaleNames : StringTables.FemaleNames).Pick(),
            StringTables.Surnames.Pick(),
            m ? Gender.Male : Gender.Female,
            FakeBirth(minAge, maxAge));
    }

    /// <summary>
    /// Generates a completely random person.
    /// </summary>
    /// <returns>
    /// A completely random person.
    /// </returns>
    public static Person Someone()
    {
        return new[] { Baby, Toddler, Child, Juvenile, Adult, Old }.Pick().Invoke();
    }

    /// <summary>
    /// Generates a random date whose age in years falls within the specified range.
    /// </summary>
    /// <param name="minAge">Minimum age to generate.</param>
    /// <param name="maxAge">Maximum age to generate.</param>
    /// <returns>
    /// A random date, which when applied to a person, will provide an age that falls
    /// within the specified range of ages.
    /// </returns>
    public static DateTime FakeBirth(int minAge, int maxAge)
    {
        var a = Math.FusedMultiplyAdd(_rnd.NextDouble(), maxAge - minAge, minAge);
        return DateTime.Today - TimeSpan.FromDays(a * 365.25);
    }
}
