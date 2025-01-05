using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Contains methods for generating test data in the context of online accounts
/// and the Internet.
/// </summary>
public static class Internet
{
    private static string[] _fakeDomains;

    /// <summary>
    /// Initializes the class <see cref="Internet"/>
    /// </summary>
    static Internet()
    {
        UseDomains(null);
    }

    /// <summary>
    /// Generates a completely random email address.
    /// </summary>
    /// <returns>
    /// A valid email address. Subsequent calls to this method can obtain
    /// email addresses from the same domain.
    /// </returns>
    public static string FakeEmail()
    {
        return $"{FakeUsername()}@{_fakeDomains.Pick()}";
    }

    /// <summary>
    /// Generates a random email address for the specified person object.
    /// </summary>
    /// <param name="person">
    /// The person for whom to generate the email address.
    /// </param>
    /// <returns>
    /// A valid email address. Subsequent calls to this method can obtain
    /// email addresses from the same domain.
    /// </returns>
    public static string FakeEmail(Person? person)
    {
        return $"{FakeUsername(person)}@{_fakeDomains.Pick()}";
    }

    /// <summary>
    /// Generates a completely random username.
    /// </summary>
    /// <returns>A completely random username.</returns>
    public static string FakeUsername()
    {
        var sb = new StringBuilder();
        var rounds = _rnd.Next(1, 4);
        sb.Append(StringTables.Lorem.Pick());
        do
        {
            if (_rnd.CoinFlip()) sb.Append("-_.".Pick());
            sb.Append(new[] { StringTables.Lorem.Pick(), _rnd.Next(0, 10000).ToString().PadLeft(_rnd.Next(1, 5), '0') }.Pick());
        } while (--rounds > 0);
        return sb.ToString();
    }

    /// <summary>
    /// Generates a random username suitable for the specified person.
    /// </summary>
    /// <param name="person">
    /// The person for whom to generate a random username.
    /// </param>
    /// <returns>A randomly generated username based on the properties of the specified person.</returns>
    public static string FakeUsername(Person? person)
    {
        person ??= Person.Someone();
        var sb = new StringBuilder();
        var rounds = _rnd.Next(1, 4);
        sb.Append(new[] { person.Surname, person.FirstName, StringTables.Lorem.Pick() }.Pick());
        do
        {
            if (_rnd.CoinFlip()) sb.Append('_');
            sb.Append(new[] { StringTables.Lorem.Pick(), person.Birth.Year.ToString(), person.Birth.Year.ToString()[2..], _rnd.Next(0, 1000).ToString().PadLeft(_rnd.Next(1, 4), '0') }.Pick());
        } while (--rounds > 0);
        return sb.ToString().ToLower();
    }

    /// <summary>
    /// Creates a new domain name given the specified component names.
    /// </summary>
    /// <param name="names">
    /// Names to use for generating the domain name.
    /// </param>
    /// <returns>
    /// A string with a domain name.
    /// </returns>
    public static string NewDomain(params string[] names) => NewDomain(names.AsEnumerable());

    /// <summary>
    /// Creates a new domain name given the specified component names.
    /// </summary>
    /// <param name="names">
    /// Names to use for generating the domain name.
    /// </param>
    /// <returns>
    /// A string with a domain name.
    /// </returns>
    public static string NewDomain(IEnumerable<string> names)
    {
        return NewDomain(names, null);
    }

    /// <summary>
    /// Creates a new domain name given the specified component names.
    /// </summary>
    /// <param name="names">
    /// Names to use for generating the domain name.
    /// </param>
    /// <param name="countryHint">
    /// Address containing a country hint for the domain name.
    /// </param>
    /// <returns>
    /// A string with a domain name.
    /// </returns>
    public static string NewDomain(IEnumerable<string> names, Address? countryHint)
    {
        string[] top = ["com", "net", "edu", "gov", "org", "info", "io"];
        var ctop = countryHint?.Country is not null ? GetTopDomainForCountry(countryHint.Country) : GetRandomCultureTopDomain();
        return $"{string.Concat(names)}.{top.Pick()}{(_rnd.CoinFlip() ? $".{ctop}" : null)}".ToLower().Replace(" ", "");
    }

    /// <summary>
    /// Tells the domain generator to use the specified domains.
    /// </summary>
    /// <param name="domainNames">
    /// Domains to be used when generating random URLs/email addresses.
    /// </param>
    [MemberNotNull(nameof(_fakeDomains))]
    public static void UseDomains(IEnumerable<string?>? domainNames)
    {
        _fakeDomains = (domainNames?.NotNull() ?? GetFauxDomains(15)).ToArray();
    }

    /// <summary>
    /// Tells the domain generator to use automatically generated fake domains.
    /// </summary>
    /// <param name="count">Number of domains to generate.</param>
    public static void UseFauxDomains(in int count)
    {
        UseDomains(GetFauxDomains(count));
    }

    /// <summary>
    /// Gets a list of fake web domain names.
    /// </summary>
    /// <param name="count">Number of domains to generate.</param>
    /// <returns>
    /// A sequence of randomly generated fake domain names.
    /// </returns>
    public static IEnumerable<string> GetFauxDomains(in int count)
    {
        if (count <= 0) ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        return Enumerable.Range(0, count).Select(_ => NewDomain(GetName(), GetName()));
    }

    private static string GetRandomCultureTopDomain()
    {
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Except([CultureInfo.InvariantCulture])
            .Where(p => !p.IsNeutralCulture)
            .Select(p => p.TwoLetterISOLanguageName.ToLower())
            .Distinct()
            .ToArray().Pick();
    }

    private static string GetTopDomainForCountry(string countryName)
    {
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(x => new RegionInfo(x.Name))
            .FirstOrDefault(region => region.EnglishName.Contains(countryName))?.TwoLetterISORegionName ?? GetRandomCultureTopDomain();
    }

    private static string GetName()
    {
        return new[] { StringTables.MaleNames, StringTables.FemaleNames, StringTables.Surnames, StringTables.Lorem }.Pick().Pick();
    }
}
