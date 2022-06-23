using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.MCART.Types.Extensions.RandomExtensions;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Contiene métodos de generación y las propiedades de una compañía ficticia.
/// </summary>
public class Company
{
    /// <summary>
    /// Obtiene un nombre para la compañía.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Obtiene una dirección para la compañía.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Obtiene un nombre de dominio para la compañía.
    /// </summary>
    public string DomainName { get; }

    /// <summary>
    /// Obtiene una URL para la compañía.
    /// </summary>
    public string Website => $"https://www.{DomainName}/";

    /// <summary>
    /// inicializa una nueva instancia de la clase <see cref="Company"/>.
    /// </summary>
    public Company()
    {
        var n1 = GetName();
        var n2 = _rnd.CoinFlip() ? $"{(_rnd.CoinFlip() ? "& " : null)}{GetName()}" : null;

        Name = string.Join(" ", new[] {
            Capitalize(n1),
            n2 is not null ? Capitalize(n2) : null,
            new[]{ "Co.", "Inc.", "LLC", "Ltd.", "Corp." }.Pick()
        }.NotNull());
        Address = Address.NewAddress();
        DomainName = Internet.NewDomain(new[] { n1, n2?.Replace("& ", "and") }.NotNull());
    }

    private static string GetName()
    {
        return new[] { StringTables.MaleNames, StringTables.FemaleNames, StringTables.Surnames, StringTables.Lorem }.Pick().Pick();
    }
}
