using System.Runtime.CompilerServices;
using TheXDS.MCART.Resources;

namespace TheXDS.Triton.Faker.Resources;

internal static class StringTables
{
    private static readonly StringUnpacker _u = new(typeof(StringTables).Assembly, typeof(StringTables).FullName!);

    private static string[]? _femaleNames = null;
    private static string[]? _maleNames = null;
    private static string[]? _surnames = null;
    private static string[]? _lorem = null;
    private static string[]? _chiefPositions = null;
    private static string[]? _workPositions = null;

    public static string[] FemaleNames => Unpack(ref _femaleNames);

    public static string[] MaleNames => Unpack(ref _maleNames);

    public static string[] Surnames => Unpack(ref _surnames);

    public static string[] Lorem => Unpack(ref _lorem);

    public static string[] ChiefPositions => Unpack(ref _chiefPositions);

    public static string[] WorkPositions => Unpack(ref _workPositions);

    private static string[] Unpack(ref string[]? array, [CallerMemberName]string id = null!)
    {
        return array ??= _u.Unpack(id.ToLower(), new DeflateGetter()).Split(',');
    }
}
