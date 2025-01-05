using System.Globalization;
using System.Runtime.CompilerServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Faker.Globals;

namespace TheXDS.Triton.Faker;

/// <summary>
/// Object that describes a complete physical location.
/// </summary>
public record Address(string AddressLine, string? AddressLine2, string City, string Country, int Zip)
{
    private static readonly string[] directions = ["N", "NE", "E", "SE", "S", "SW", "W", "NW"];
    private static readonly string[] roadTypes = ["Ave.", "Road", "Street", "Highway"];
    private static readonly string[] unitType = ["#", "Apt.", "House", "Building"];

    /// <inheritdoc/>
    public override string ToString()
    {
        return $@"{string.Join(Environment.NewLine, new[] { AddressLine, AddressLine2, $"{City}, {Country} {Zip}" }.NotNull())}";
    }

    /// <summary>
    /// Generates a random physical address.
    /// </summary>
    /// <returns>A random physical address.</returns>
    public static Address NewAddress()
    {
        static string RndAddress()
        {
            var l = new List<string>();
            if (_rnd.CoinFlip()) l.Add(_rnd.Next(1, 300).ToString());
            if (_rnd.CoinFlip())
            {
                l.Add(directions.Pick());
            }

            l.Add(_rnd.CoinFlip() ? Capitalize(StringTables.Surnames.Pick()) : GetOrdinal(_rnd.Next(1, 130)));
            l.Add(roadTypes.Pick());
            return string.Join(' ', l);
        }
        static string? RndLine2()
        {
            return _rnd.CoinFlip() ? $"{unitType.Pick()} {_rnd.Next(1, 9999)}" : null;
        }
        static string RndCity() => string.Join(' ', new[] { Capitalize(StringTables.Surnames.Pick()), _rnd.CoinFlip() ? "City" : null }.NotNull());
        return new(RndAddress(), RndLine2(), RndCity(), RandomCountry(), _rnd.Next(10001, 99999));
    }

    /// <summary>
    /// Gets a random country name.
    /// </summary>
    /// <returns>
    /// The name of a real country known to the system, selected randomly.
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
