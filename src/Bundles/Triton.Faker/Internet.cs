using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.MCART.Types.Extensions.RandomExtensions;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Contiene funciones de generación de datos de pruebas en el contexto de
/// cuentas en línea e internet.
/// </summary>
public class Internet
{
    private static string[]? fakeDomains;

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
        return $"{FakeUsername()}@{(fakeDomains ??= LoadDomains()).Pick()}";
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

    private static string[] LoadDomains()
    {
        return Enumerable.Range(0, 15).Select(_ => NewDomain(GetName(), GetName())).ToArray();
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
        string[] top = { "com", "net", "edu", "gov", "org", "info", "io" };
        string[] ctop = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Except(new[] { CultureInfo.InvariantCulture })
            .Where(p => !p.IsNeutralCulture)
            .Select(p => p.TwoLetterISOLanguageName.ToLower())
            .Distinct()
            .ToArray();
        return $"{string.Concat(names)}.{top.Pick()}{(_rnd.CoinFlip() ? $".{ctop.Pick()}" : null)}".ToLower().Replace(" ", "");
    }

    private static string GetName()
    {
        return new[] { StringTables.MaleNames, StringTables.FemaleNames, StringTables.Surnames, StringTables.Lorem }.Pick().Pick();
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
        return $"{FakeUsername(person)}@{(fakeDomains ??= LoadDomains()).Pick()}";
    }
}
