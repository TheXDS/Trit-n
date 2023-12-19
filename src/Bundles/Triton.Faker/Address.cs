using System.Globalization;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Objeto que describe una ubicación física completa.
/// </summary>
public record Address(string AddressLine, string? AddressLine2, string City, string Country, int Zip)
{
    /// <inheritdoc/>
    public override string ToString()
    {
        return $@"{string.Join(Environment.NewLine, new[] { AddressLine, AddressLine2, $"{City}, {Country} {Zip}" }.NotNull())}";
    }

    /// <summary>
    /// Genera una dirección física aleatoria.
    /// </summary>
    /// <returns>Una dirección física aleatoria.</returns>
    public static Address NewAddress()
    {
        static string RndAddress()
        {
            var l = new List<string>();
            if (_rnd.CoinFlip()) l.Add(_rnd.Next(1, 300).ToString());
            if (_rnd.CoinFlip()) l.Add(new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" }.Pick());
            l.Add(_rnd.CoinFlip() ? Capitalize(StringTables.Surnames.Pick()) : GetOrdinal(_rnd.Next(1, 130)));
            l.Add(new[] { "Ave.", "Road", "Street", "Highway" }.Pick());
            return string.Join(' ', l);
        }
        static string? RndLine2() => _rnd.CoinFlip() ? $"{new[] { "#", "Apt.", "House", "Building" }.Pick()} {_rnd.Next(1, 9999)}" : null;
        static string RndCity() => string.Join(' ', new [] { Capitalize(StringTables.Surnames.Pick()), _rnd.CoinFlip() ? "City" : null }.NotNull());
        return new(RndAddress(), RndLine2(), RndCity(), RandomCountry(), _rnd.Next(10001, 99999));
    }

    /// <summary>
    /// Obtiene un nombre de país aleatorio.
    /// </summary>
    /// <returns>
    /// El nombre de un país real conocido por el sistema, seleccionado de
    /// forma aleatoria.
    /// </returns>
    public static string RandomCountry()
    {
        return new RegionInfo(CultureInfo.GetCultures(CultureTypes.SpecificCultures).Pick().Name).EnglishName;
    }

    private static string GetOrdinal(int value)
    {
        var l = value.ToString().PadLeft(2, '0')[..2];
        return value.ToString().Last() switch
        {
            '1' when l != "11" => $"{value}st",
            '2' when l != "12" => $"{value}nd",
            '3' when l != "13" => $"{value}rd",
            _ => $"{value}th"
        };
    }
}
