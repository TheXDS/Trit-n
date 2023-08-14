using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Contiene funciones de generación de datos de pruebas en el contexto de
/// cuentas en línea e internet.
/// </summary>
public static class Internet
{
    private static string[] _fakeDomains;

    /// <summary>
    /// Inicializa la clase <see cref="Internet"/>
    /// </summary>
    static Internet()
    {
        UseDomains(null);
    }

    /// <summary>
    /// Genera una dirección de correo totalmente aleatoria.
    /// </summary>
    /// <returns>
    /// Una dirección de correo con un formato válido. Subsecuentes
    /// llamadas a este método podrán obtener direcciones de correo del
    /// mismo dominio.
    /// </returns>
    public static string FakeEmail()
    {
        return $"{FakeUsername()}@{_fakeDomains.Pick()}";
    }

    /// <summary>
    /// Genera una dirección de correo electrónico aleatoria para el objeto
    /// <see cref="Person"/> especificado.
    /// </summary>
    /// <param name="person">
    /// Persona para la cual generar la dirección de correo.
    /// </param>
    /// <returns>
    /// Una dirección de correo con un formato válido. Subsecuentes
    /// llamadas a este método podrán obtener direcciones de correo del
    /// mismo dominio.
    /// </returns>
    public static string FakeEmail(Person? person)
    {
        return $"{FakeUsername(person)}@{_fakeDomains.Pick()}";
    }

    /// <summary>
    /// Genera un nombre de usuario totalmente aleatorio.
    /// </summary>
    /// <returns>Un nombre de usuario totalmente aleatorio.</returns>
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
    /// Genera un nombre de usuario aleatorio satisfactorio para la persona
    /// especificada.
    /// </summary>
    /// <param name="person">
    /// Persona para la cual generar un nombre de usuario aleatorio.
    /// </param>
    /// <returns>
    /// Un nombre de usuario aleatorio basado en las propiedades de la 
    /// persona especificada.
    /// </returns>
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
    /// Crea un nuevo nombre de dominio dados los componentes de nombres
    /// especificados.
    /// </summary>
    /// <param name="names">
    /// Nombres a utilizar para generar el nombre de dominio.
    /// </param>
    /// <returns>
    /// Una cadena con un nombre de dominio.
    /// </returns>
    public static string NewDomain(params string[] names) => NewDomain(names.AsEnumerable());

    /// <summary>
    /// Crea un nuevo nombre de dominio dados los componentes de nombres especificados.
    /// </summary>
    /// <param name="names">
    /// Nombres a utilizar para generar el nombre de dominio.
    /// </param>
    /// <returns>
    /// Una cadena con un nombre de dominio.
    /// </returns>
    public static string NewDomain(IEnumerable<string> names)
    {
        return NewDomain(names, null);
    }

    /// <summary>
    /// Crea un nuevo nombre de dominio dados los componentes de nombres
    /// especificados.
    /// </summary>
    /// <param name="names">
    /// Nombres a utilizar para generar el nombre de dominio.
    /// </param>
    /// <param name="countryHint">
    /// Dirección que contiene una sugerencia de país para el nombre de
    /// dominio.
    /// </param>
    /// <returns>
    /// Una cadena con un nombre de dominio.
    /// </returns>
    public static string NewDomain(IEnumerable<string> names, Address? countryHint)
    {
        string[] top = { "com", "net", "edu", "gov", "org", "info", "io" };
        var ctop = countryHint?.Country is not null ? GetTopDomainForCountry(countryHint.Country) : GetRandomCultureTopDomain();
        return $"{string.Concat(names)}.{top.Pick()}{(_rnd.CoinFlip() ? $".{ctop}" : null)}".ToLower().Replace(" ", "");
    }

    /// <summary>
    /// Indica al generador de dominios que debe utilizar los dominios especificados.
    /// </summary>
    /// <param name="domainNames"></param>
    [MemberNotNull(nameof(_fakeDomains))]
    public static void UseDomains(IEnumerable<string?>? domainNames)
    {
        _fakeDomains = (domainNames.NotNull().OrNull() ?? GetFauxDomains(15)).ToArray();
    }

    /// <summary>
    /// Indica al generador de dominios que debe utilizar dominios falsos
    /// generados automáticamente.
    /// </summary>
    /// <param name="count">Cantidad de dominios a generar.</param>
    public static void UseFauxDomains(in int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        UseDomains(GetFauxDomains(count));
    }

    /// <summary>
    /// Obtiene una lista de dominios web falsos.
    /// </summary>
    /// <param name="count">Cantidad de dominios a generar.</param>
    /// <returns>
    /// Un arreglo de dominios falsos generados aleatoriamente.
    /// </returns>
    public static IEnumerable<string> GetFauxDomains(in int count)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        return Enumerable.Range(0, count).Select(_ => NewDomain(GetName(), GetName()));
    }

    private static string GetRandomCultureTopDomain()
    {
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Except(new[] { CultureInfo.InvariantCulture })
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
