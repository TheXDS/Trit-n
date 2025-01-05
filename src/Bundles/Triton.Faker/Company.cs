using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Contains methods for generation and properties of a fictional company.
/// </summary>
public class Company
{
    private static readonly string[] orgType = ["Co.", "Inc.", "LLC", "Ltd.", "Corp.", "GmbH", "S.A.", "S.L.", "S.R.L."];

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the address of the company.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Gets a domain name for the company.
    /// </summary>
    public string DomainName { get; }

    /// <summary>
    /// Gets a URL for the company.
    /// </summary>
    public string Website => $"https://www.{DomainName}/";

    /// <summary>
    /// Gets an employee randomly.
    /// </summary>
    /// <returns>
    /// A new instance of the <see cref="Employee"/> class.
    /// </returns>
    public Employee RndEmployee()
    {
        return Employee.Get(this);
    }

    /// <summary>
    /// Gets a chief employee randomly.
    /// </summary>
    /// <returns>
    /// A new instance of the <see cref="Employee"/> class.
    /// </returns>
    public Employee RndChief()
    {
        return Employee.GetChief(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Company"/> class.
    /// </summary>
    public Company()
    {
        var n1 = GetName();
        var n2 = _rnd.CoinFlip() ? $"{(_rnd.CoinFlip() ? "& " : null)}{Capitalize(GetName())}" : null;

        Name = string.Join(" ", (new[] {
            Capitalize(n1),
            n2 is not null ? n2 : null,
            orgType.Pick()
        }).NotNull());
        Address = Address.NewAddress();
        DomainName = Internet.NewDomain(new[] { n1, n2?.Replace("& ", "and").ToLower() }.NotNull(), Address);
    }

    private static string GetName()
    {
        return new[] { StringTables.MaleNames, StringTables.FemaleNames, StringTables.Surnames, StringTables.Lorem }.Pick().Pick();
    }
}
